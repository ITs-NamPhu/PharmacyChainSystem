import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { MdAddCircle } from "react-icons/md";

import ModalCreateGoodsReceipt from './ModalCreateGoodsReceipt';
import ModalUpdateGoodsReceipt from './ModalUpdateGoodsReceipt';
import ModalDeleteGoodsReceipt from './ModalDeleteGoodsReceipt';
import TableGoodsReceipt from './tableGoodsReceipt';
import SearchBar from '../common/SearchBar';
import FilterPanel, { FilterDateRange, FilterNumber } from '../common/FilterPanel';

import './IndexManageGoodsReceipt.scss';

import { getGoodsReceiptFiltered } from '../../services/apiService';
import { useGoodsReceiptFilter } from '../../stores/filterStore';

const IndexManageGoodsReceipt = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const { filters, setFilter, setFilters, resetFilters, hydrateFromUrl, toUrlParams } = useGoodsReceiptFilter();

    const [listGoodsReceipt, setListGoodsReceipt] = useState([]);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateGoodsReceipt, setShowModalCreateGoodsReceipt] = useState(false);
    const [showModalUpdateGoodsReceipt, setShowModalUpdateGoodsReceipt] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});
    const [showModalDeleteGoodsReceipt, setShowModalDeleteGoodsReceipt] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        hydrateFromUrl(searchParams);
    }, []);

    useEffect(() => {
        fetchListGoodsReceipt(filters);
    }, [filters]);

    const fetchListGoodsReceipt = async (f) => {
        const params = { ...f };
        let res = await getGoodsReceiptFiltered(params);
        if (res.ec === 0) {
            setListGoodsReceipt(res.dt.goodsReceipts);
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
            <div className="title">Quản lý phiếu nhập hàng</div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => setShowModalCreateGoodsReceipt(true)}>
                        <MdAddCircle /> Thêm phiếu nhập
                    </button>
                </div>

                <SearchBar
                    value={filters.keyword || ''}
                    onSearch={(val) => setFilter('keyword', val || undefined)}
                    placeholder="Tìm theo số phiếu hoặc tên NCC..."
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
                    <FilterDateRange
                        labelFrom="Từ ngày"
                        labelTo="Đến ngày"
                        valueFrom={filters.fromDate}
                        valueTo={filters.toDate}
                        onChangeFrom={(val) => setFilter('fromDate', val)}
                        onChangeTo={(val) => setFilter('toDate', val)}
                    />
                    <FilterNumber
                        label="Từ tiền"
                        value={filters.minTotalAmount}
                        min={0}
                        onChange={(val) => setFilter('minTotalAmount', val)}
                    />
                    <FilterNumber
                        label="Đến tiền"
                        value={filters.maxTotalAmount}
                        min={0}
                        onChange={(val) => setFilter('maxTotalAmount', val)}
                    />
                </FilterPanel>

                <div className='table-container'>
                    <TableGoodsReceipt
                        fetchListGoodsReceipt={handlePageChange}
                        listGoodsReceipt={listGoodsReceipt}
                        handleUpdateGoodsReceipt={(gr) => { setShowModalUpdateGoodsReceipt(true); setDataUpdate(gr); }}
                        handleDeleteGoodsReceipt={(gr) => { setShowModalDeleteGoodsReceipt(true); setDataDelete(gr); }}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />
                </div>

                <ModalCreateGoodsReceipt show={showModalCreateGoodsReceipt} setShow={setShowModalCreateGoodsReceipt} fetchListGoodsReceipt={() => fetchListGoodsReceipt(filters)} />
                <ModalUpdateGoodsReceipt show={showModalUpdateGoodsReceipt} setShow={setShowModalUpdateGoodsReceipt} fetchListGoodsReceipt={() => fetchListGoodsReceipt(filters)} dataUpdate={dataUpdate} />
                <ModalDeleteGoodsReceipt show={showModalDeleteGoodsReceipt} setShow={setShowModalDeleteGoodsReceipt} fetchListGoodsReceipt={() => fetchListGoodsReceipt(filters)} dataDelete={dataDelete} setCurrentPage={setCurrentPage} />
            </div>
        </div>
    );
}
export default IndexManageGoodsReceipt;
