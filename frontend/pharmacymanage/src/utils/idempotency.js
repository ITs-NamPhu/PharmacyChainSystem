import { v5 as uuidv5 } from "uuid";
import { store } from '../redux/store';

const IDEMPOTENCY_NAMESPACE = '1b671a64-40d5-491e-99b0-da01ff1f3341';
export const MUTATING_METHODS = ['post', 'put', 'delete', 'patch'];

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

export const computeIdempotencyKey = (config) => {
    const account = store?.getState().user?.account;
    const raw = stableStringify({
        method: (config.method || 'get').toLowerCase(),
        url: config.url || '',
        params: config.params || null,
        data: config.data ?? null,
        branchId: account?.currentBranchId ?? '',
        userId: account?.userId ?? ''
    });

    try {
        return uuidv5(raw, IDEMPOTENCY_NAMESPACE);
    } catch (err) {
        let hash = 5381;
        for (let i = 0; i < raw.length; i++) {
            hash = ((hash << 5) + hash + raw.charCodeAt(i)) >>> 0;
        }
        return `idem-${hash.toString(36)}`;
    }
};
