import { create } from 'zustand';

const cleanParams = (obj) => {
    const result = {};
    for (const [key, value] of Object.entries(obj)) {
        if (value !== null && value !== undefined && value !== '' && value !== 'All' && value !== 0) {
            result[key] = value;
        }
    }
    return result;
};

const createFilterStore = (initialFilters = {}) => create((set, get) => ({
    filters: { pageNumber: 1, pageSize: 10, isDescending: true, ...initialFilters },

    setFilter: (key, value) =>
        set((state) => ({
            filters: { ...state.filters, [key]: value, pageNumber: key === 'pageNumber' ? value : 1 },
        })),

    setFilters: (patch) =>
        set((state) => ({
            filters: { ...state.filters, ...patch, pageNumber: 1 },
        })),

    resetFilters: () =>
        set({ filters: { pageNumber: 1, pageSize: 10, isDescending: true, ...initialFilters } }),

    toUrlParams: () => {
        const params = new URLSearchParams();
        const cleaned = cleanParams(get().filters);
        for (const [key, value] of Object.entries(cleaned)) {
            params.set(key, String(value));
        }
        return params;
    },

    hydrateFromUrl: (searchParams) => {
        if (!searchParams) return;
        const params = {};
        searchParams.forEach((value, key) => {
            if (key === 'pageSize' || key === 'pageNumber') {
                params[key] = parseInt(value, 10) || (key === 'pageSize' ? 10 : 1);
            } else if (key === 'isDescending') {
                params[key] = value === 'true';
            } else if (key === 'stockStatus' || key === 'expiryStatus') {
                params[key] = parseInt(value, 10) || 0;
            } else {
                params[key] = value;
            }
        });
        set((state) => ({
            filters: { ...state.filters, ...params },
        }));
    },
}));

export const useMedicineFilter = createFilterStore({ stockStatus: 0 });
export const useBatchFilter = createFilterStore({ expiryStatus: 0 });
export const useInvoiceFilter = createFilterStore({});
export const useGoodsReceiptFilter = createFilterStore({});
