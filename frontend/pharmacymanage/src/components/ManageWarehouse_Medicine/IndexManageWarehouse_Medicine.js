import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import TableWarehouse_Medicine from './TableWarehouse_Medicine';
import SearchBar from '../common/SearchBar';
import FilterPanel, { FilterSelect } from '../common/FilterPanel';
import './IndexManageWarehouse_Medicine.scss';
import { getBatchFiltered } from '../../services/apiService';
import { useBatchFilter } from '../../stores/filterStore';

const EXPIRY_STATUS_OPTIONS = [
    { value: 0, label: 'Tất cả' },
    { value: 1, label: 'Cận date ≤ 3 tháng' },
    { value: 2, label: 'Cận date ≤ 6 tháng' },
    { value: 3, label: 'Đã hết hạn' },
];

const IndexManageWarehouse_Medicine = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const { filters, setFilter, setFilters, resetFilters, hydrateFromUrl, toUrlParams } = useBatchFilter();

    const [listBatch, setListBatch] = useState([]);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        hydrateFromUrl(searchParams);
    }, []);

    useEffect(() => {
        fetchListBatch(filters);
    }, [filters]);

    const fetchListBatch = async (f) => {
        const params = { ...f };
        params.expiryStatus = Number(params.expiryStatus || 0);
        let res = await getBatchFiltered(params);
        if (res.ec === 0) {
            setListBatch(res.dt.batches);
            setPageCount(res.dt.totalPage);
            setCurrentPage(f.pageNumber || 1);
        }
        syncToUrl();
    };

    const syncToUrl = useCallback(() => {
        const params = toUrlParams();
        const current = searchParams.toString();
        const next = params.toString();
        if (current !== next) {
            setSearchParams(params, { replace: true });
        }
    }, [filters]);

    const handlePageChange = (page) => {
        setFilter('pageNumber', page);
    };

    return (
        <div className='manage-container'>
            <div className="title">Kho thuốc</div>
            <div className='user-content'>
                <SearchBar
                    value={filters.keyword || ''}
                    onSearch={(val) => setFilter('keyword', val || undefined)}
                    placeholder="Tìm theo tên thuốc..."
                />

                <FilterPanel
                    filters={filters}
                    onFilterChange={(key, val) => {
                        if (key === '__apply') {
                            setFilters({ ...filters, pageNumber: 1 });
                            return;
                        }
                        setFilter(key, val);
                    }}
                    onReset={resetFilters}
                >
                    <FilterSelect
                        label="Trạng thái hạn dùng"
                        value={filters.expiryStatus ?? 0}
                        options={EXPIRY_STATUS_OPTIONS}
                        onChange={(val) => setFilter('expiryStatus', Number(val))}
                    />
                </FilterPanel>

                <div className='table-container'>
                    <TableWarehouse_Medicine
                        fetchListBatch={handlePageChange}
                        listBatch={listBatch}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />
                </div>
            </div>
        </div>
    );
}
export default IndexManageWarehouse_Medicine;
