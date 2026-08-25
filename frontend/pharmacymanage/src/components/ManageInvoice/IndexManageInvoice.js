import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { MdAddCircle } from "react-icons/md";

import ModalCreateInvoice from './ModalCreateInvoice';
import ModalUpdateInvoice from './ModalUpdateInvoice';
import ModalDeleteInvoice from './ModalDeleteInvoice';
import TableInvoice from './tableInvoice';
import SearchBar from '../common/SearchBar';
import FilterPanel, { FilterDateRange, FilterNumber } from '../common/FilterPanel';

import './IndexManageInvoice.scss';

import { getInvoiceFiltered } from '../../services/apiService';
import { useInvoiceFilter } from '../../stores/filterStore';

const IndexManageInvoice = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const { filters, setFilter, setFilters, resetFilters, hydrateFromUrl, toUrlParams } = useInvoiceFilter();

    const [listInvoice, setListInvoice] = useState([]);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateInvoice, setShowModalCreateInvoice] = useState(false);
    const [showModalUpdateInvoice, setShowModalUpdateInvoice] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});
    const [showModalDeleteInvoice, setShowModalDeleteInvoice] = useState(false);
    const [dataDelete, setDataDelete] = useState({});
    // chạy 1 lần mục đích đọc URL ban đầu (hoặc khi user mở trang hoặc F5) và đổ vào store
    useEffect(() => {
        hydrateFromUrl(searchParams);
    }, []);

    useEffect(() => {
        fetchListInvoice(filters);
    }, [filters]);

    const fetchListInvoice = async (f) => {
        const params = { ...f };
        if (params.customerID) params.customerID = Number(params.customerID);
        if (params.userID) params.userID = Number(params.userID);
        let res = await getInvoiceFiltered(params);
        if (res.ec === 0) {
            setListInvoice(res.dt.invoices);
            setPageCount(res.dt.totalPage);
            setCurrentPage(f.pageNumber || 1);
        }
        syncToUrl();
    };

    // khi filters trong store zustand thay đổi cập nhật hook searchParams
    // toUrlParams() giúp URL clean
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
            <div className="title">Quản lý hóa đơn bán hàng</div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => setShowModalCreateInvoice(true)}>
                        <MdAddCircle /> Thêm hóa đơn
                    </button>
                </div>

                <SearchBar
                    value={filters.keyword || ''}
                    onSearch={(val) => setFilter('keyword', val || undefined)}
                    placeholder="Tìm theo mã HĐ hoặc tên khách hàng..."
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
                    <TableInvoice
                        fetchListInvoice={handlePageChange}
                        listInvoice={listInvoice}
                        handleUpdateInvoice={(inv) => { setShowModalUpdateInvoice(true); setDataUpdate(inv); }}
                        handleDeleteInvoice={(inv) => { setShowModalDeleteInvoice(true); setDataDelete(inv); }}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />
                </div>
                <ModalCreateInvoice show={showModalCreateInvoice} setShow={setShowModalCreateInvoice} fetchListInvoice={() => fetchListInvoice(filters)} />
                <ModalUpdateInvoice show={showModalUpdateInvoice} setShow={setShowModalUpdateInvoice} fetchListInvoice={() => fetchListInvoice(filters)} dataUpdate={dataUpdate} />
                <ModalDeleteInvoice show={showModalDeleteInvoice} setShow={setShowModalDeleteInvoice} fetchListInvoice={() => fetchListInvoice(filters)} dataDelete={dataDelete} setCurrentPage={setCurrentPage} />
            </div>
        </div>
    );
};
export default IndexManageInvoice;
