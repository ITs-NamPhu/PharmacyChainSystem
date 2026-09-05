import axios from "axios";
import nProgress from "nprogress";
import { store } from '../redux/store';
import { postRefreshToken } from "../services/apiService";
import { doRefreshToken } from "../redux/action/userAction";
import { v5 as uuidv5 } from "uuid";
// import axiosRetry from 'axios-retry';



// tạo hiệu ứng khi gửi request
nProgress.configure({
    showSpinner: false,
    trickleSpeed: 100
})


const refresh_token = async () => {
    const accessToken = store?.getState().user.account.access_token;
    const refreshToken = store?.getState().user.account.refresh_token;
    let data = { accessToken, refreshToken }
    return await postRefreshToken(data);
}


const instance = axios.create({
    baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7196'
})



// Add a request interceptor
instance.interceptors.request.use(function (config) {
    if (!config.url.includes("api/auth/refreshtoken")) {
        const access_token = store?.getState().user.account.access_token;
        config.headers["Authorization"] = "Bearer " + access_token;

        const currentBranchId = store?.getState().user.account.currentBranchId;
        if (currentBranchId) {
            config.headers["X-Branch-Id"] = currentBranchId;
        }
    }

    nProgress.start();
    return config;
}, function (error) {
    return Promise.reject(error);
});



// handle for refresh token
// Case: nếu request bị lỗi 401 (Unauthorized) for access token hết hạn
//      và chưa retry lần nào thì sẽ gọi API refresh token để lấy access token mới,
//      sau đó retry lại request cũ với access token mới.

let isRefreshing = false;
let refreshSubscribers = [];

// duyệt qua tất cả request đang chờ refresh token
// và gọi lại với access token mới
function onRefreshed(accessToken) {
    refreshSubscribers.forEach(callback => callback(accessToken));
    refreshSubscribers = [];
}

function addRefreshSubscriber(callback) {
    refreshSubscribers.push(callback);
}

// Add a response interceptor
instance.interceptors.response.use(function (response) {
    nProgress.done();
    return response && response.data ? response.data : response;
}, async function (error) {

    nProgress.done();

    const originalRequest = error.config;

    if (error.response?.data?.ec === -999 && !originalRequest._retry) {
        if (isRefreshing) {
            return new Promise((resolve) => {
                addRefreshSubscriber((accessToken) => {
                    originalRequest.headers["Authorization"] = "Bearer " + accessToken;
                    originalRequest._retry = true;
                    resolve(instance(originalRequest));
                });
            });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        // nếu gọi refresh token thành công thì thực hiện lại request cũ với access token mới
        // không thì reject lỗi reset lại queue refreshSubscribers và isRefreshing
        try {
            const res = await refresh_token();
            if (res && res.ec === 0) {
                store.dispatch(doRefreshToken(res));
                const accessToken = res.dt.accessToken;

                isRefreshing = false;
                onRefreshed(accessToken);

                originalRequest.headers["Authorization"] = "Bearer " + accessToken;
                return instance(originalRequest);
            } else {
                isRefreshing = false;
                refreshSubscribers = [];
                return Promise.reject(error);
            }
        } catch (err) {
            isRefreshing = false;
            refreshSubscribers = [];
            return Promise.reject(err);
        }
    }

    return error?.response?.data
        ? error.response.data : Promise.reject(error);
});

// ---------- Idempotency cho các request mutation (POST/PUT/DELETE/PATCH) ----------
// Tạo key từ nội dung request + ngữ cảnh user/branch. Khóa này được dùng làm:
//   1. Header "Idempotency-Key" gửi lên backend (để backend dedup/replay).
//   2. Khóa cho cơ chế dedup in-flight: chặn double-click gửi duplicate request.
//   3. Khóa cho replay cache ngắn hạn: submit trùng payload trong TTL -> trả response cũ.

const IDEMPOTENCY_NAMESPACE = '1b671a64-40d5-491e-99b0-da01ff1f3341';
const IDEMPOTENCY_TTL = 60 * 1000;        // 60 giây
const IDEMPOTENCY_MAX_ENTRIES = 64;
const MUTATING_METHODS = ['post', 'put', 'delete', 'patch'];

const stableStringify = (value) => {
    if (value === null || value === undefined) return 'null';
    if (typeof value !== 'object') return JSON.stringify(value);
    if (value instanceof Date) return JSON.stringify(value.toISOString());
    if (typeof FormData !== 'undefined' && value instanceof FormData) {
        return stableStringify(Object.fromEntries(value.entries()));
    }
    if (Array.isArray(value)) {
        return '[' + value.map(stableStringify).join(',') + ']';
    }
    const keys = Object.keys(value).sort();
    return '{' + keys.map((key) => JSON.stringify(key) + ':' + stableStringify(value[key])).join(',') + '}';
};

const computeIdempotencyKey = (config) => {
    const account = store?.getState().user?.account;
    const raw = stableStringify({
        method: (config.method || 'get').toLowerCase(),
        url: config.url || '',
        params: config.params || null,
        data: config.data ?? null,
        branchId: account?.currentBranchId ?? '',
        userId: account?.userId ?? ''
    });
    let key = raw;
    try {
        key = uuidv5(raw, IDEMPOTENCY_NAMESPACE);
    } catch (err) {
        // Fallback: băm djb2 nếu uuid không khả dụng
        let hash = 5381;
        for (let i = 0; i < raw.length; i++) {
            hash = ((hash << 5) + hash + raw.charCodeAt(i)) >>> 0;
        }
        key = `idem-${hash.toString(36)}`;
    }
    return key;
};

const idemInFlight = new Map();
const idemReplayCache = new Map();

const evictOldestReplay = () => {
    if (idemReplayCache.size < IDEMPOTENCY_MAX_ENTRIES) return;
    let oldestKey = null;
    let oldestTs = Infinity;
    for (const [key, entry] of idemReplayCache) {
        if (entry.ts < oldestTs) {
            oldestTs = entry.ts;
            oldestKey = key;
        }
    }
    if (oldestKey) idemReplayCache.delete(oldestKey);
};

const originalRequest = instance.request.bind(instance);

instance.request = function (config) {
    const method = (config.method || 'get').toLowerCase();
    const url = config.url || '';

    const isMutating = MUTATING_METHODS.includes(method);
    const skipByUrl = url.includes('api/auth/refreshtoken');

    if (isMutating && !skipByUrl && config.skipIdempotency !== true) {
        const key = computeIdempotencyKey(config);

        config.headers = config.headers || {};
        config.headers['Idempotency-Key'] = key;

        const hit = idemReplayCache.get(key);
        if (hit) {
            if (Date.now() - hit.ts < IDEMPOTENCY_TTL) {
                return Promise.resolve(hit.value);
            }
            idemReplayCache.delete(key);
        }

        const pending = idemInFlight.get(key);
        if (pending) return pending;

        const promise = originalRequest(config);

        idemInFlight.set(key, promise);
        promise
            .then((value) => {
                idemReplayCache.set(key, { value, ts: Date.now() });
                evictOldestReplay();
            })
            .catch(() => { /* không cache lỗi, cho phép retry */ })
            .finally(() => {
                idemInFlight.delete(key);
            });

        return promise;
    }

    return originalRequest(config);
};

export default instance;
