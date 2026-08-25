import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { MdAddCircle } from "react-icons/md";

import ModalCreateMedicine from './ModalCreateMedicine';
import ModalUpdateMedicine from './ModalUpdateMedicine';
import ModalDeleteMedicine from './ModalDeleteMedicine';
import TableMedicine from './tableMedicine';
import SearchBar from '../common/SearchBar';
import FilterPanel, { FilterSelect } from '../common/FilterPanel';

import './IndexManageMedicine.scss';

import { getMedicineFiltered, getAllMedicineCategoryAll, getAllManufacturerAll } from '../../services/apiService';
import { useMedicineFilter } from '../../stores/filterStore';

const STOCK_STATUS_OPTIONS = [
    { value: 0, label: 'Tất cả' },
    { value: 3, label: 'Hết hàng (0)' },
    { value: 2, label: 'Tồn thấp' },
    { value: 1, label: 'Còn hàng' },
];

const IndexManageMedicine = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const { filters, setFilter, setFilters, resetFilters, hydrateFromUrl, toUrlParams } = useMedicineFilter();

    const [listMedicine, setListMedicine] = useState([]);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [listCategory, setListCategory] = useState([]);
    const [listManufacturer, setListManufacturer] = useState([]);

    const [showModalCreateMedicine, setShowModalCreateMedicine] = useState(false);
    const [showModalUpdateMedicine, setShowModalUpdateMedicine] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});
    const [showModalDeleteMedicine, setShowModalDeleteMedicine] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        hydrateFromUrl(searchParams);
        loadDropdowns();
    }, []);

    useEffect(() => {
        fetchListMedicine(filters);
    }, [filters]);

    const loadDropdowns = async () => {
        const [catRes, mfgRes] = await Promise.all([
            getAllMedicineCategoryAll(),
            getAllManufacturerAll()
        ]);
        if (catRes.ec === 0) setListCategory(catRes.dt || []);
        if (mfgRes.ec === 0) setListManufacturer(mfgRes.dt || []);
    };

    const fetchListMedicine = async (f) => {
        const params = { ...f };
        if (params.categoryID) params.categoryID = Number(params.categoryID);
        if (params.manufacturerID) params.manufacturerID = Number(params.manufacturerID);
        params.stockStatus = Number(params.stockStatus || 0);

        let res = await getMedicineFiltered(params);
        if (res.ec === 0) {
            setListMedicine(res.dt.medicines);
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

    const handleResetFilters = () => {
        resetFilters();
    };

    return (
        <div className='manage-container'>
            <div className="title">Quản lý thuốc</div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => setShowModalCreateMedicine(true)}>
                        <MdAddCircle /> Thêm thuốc
                    </button>
                </div>

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
                    onReset={handleResetFilters}
                >
                    <FilterSelect
                        label="Danh mục"
                        value={filters.categoryID || ''}
                        options={[{ value: '', label: 'Tất cả' }, ...listCategory.map(c => ({ value: c.medicineCategoryID, label: c.categoryName }))]}
                        onChange={(val) => setFilter('categoryID', val || undefined)}
                    />
                    <FilterSelect
                        label="Nhà sản xuất"
                        value={filters.manufacturerID || ''}
                        options={[{ value: '', label: 'Tất cả' }, ...listManufacturer.map(m => ({ value: m.manufacturerID, label: m.manufacturerName }))]}
                        onChange={(val) => setFilter('manufacturerID', val || undefined)}
                    />
                    <FilterSelect
                        label="Trạng thái tồn kho"
                        value={filters.stockStatus ?? 0}
                        options={STOCK_STATUS_OPTIONS}
                        onChange={(val) => setFilter('stockStatus', Number(val))}
                    />
                </FilterPanel>

                <div className='table-container'>
                    <TableMedicine
                        fetchListMedicine={handlePageChange}
                        listMedicine={listMedicine}
                        handleUpdateMedicine={(m) => { setShowModalUpdateMedicine(true); setDataUpdate(m); }}
                        handleDeleteMedicine={(m) => { setShowModalDeleteMedicine(true); setDataDelete(m); }}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />
                </div>
                <ModalCreateMedicine show={showModalCreateMedicine} setShow={setShowModalCreateMedicine} fetchListMedicine={() => fetchListMedicine(filters)} />
                <ModalUpdateMedicine show={showModalUpdateMedicine} setShow={setShowModalUpdateMedicine} fetchListMedicine={() => fetchListMedicine(filters)} dataUpdate={dataUpdate} />
                <ModalDeleteMedicine show={showModalDeleteMedicine} setShow={setShowModalDeleteMedicine} fetchListMedicine={() => fetchListMedicine(filters)} dataDelete={dataDelete} setCurrentPage={setCurrentPage} />
            </div>
        </div>
    );
};
export default IndexManageMedicine;
