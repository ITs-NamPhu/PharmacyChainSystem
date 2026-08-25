import { useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';

export const useFilterSync = (filterStore) => {
    const [searchParams, setSearchParams] = useSearchParams();

    const { filters, hydrateFromUrl, toUrlParams } = filterStore();

    useEffect(() => {
        hydrateFromUrl(searchParams);
    }, []);

    const syncToUrl = useCallback(() => {
        const params = toUrlParams();
        const current = searchParams.toString();
        const next = params.toString();
        if (current !== next) {
            setSearchParams(params, { replace: true });
        }
    }, [filters, searchParams]);

    const fetchWithFilters = useCallback(
        (fetchFn) => {
            syncToUrl();
            fetchFn(filters);
        },
        [filters, syncToUrl]
    );

    return { filters, fetchWithFilters };
};
