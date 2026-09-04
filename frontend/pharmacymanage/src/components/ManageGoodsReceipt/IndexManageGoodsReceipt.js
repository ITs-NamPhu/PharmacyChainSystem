import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { MdAddCircle } from "react-icons/md";

import ModalCreateGoodsReceipt from './ModalCreateGoodsReceipt';
import ModalUpdateGoodsReceipt from './ModalUpdateGoodsReceipt';
import ModalViewGoodsReceipt from './ModalViewGoodsReceipt';
import StatusActionModal from '../common/StatusActionModal';
import TableGoodsReceipt from './tableGoodsReceipt';
import SearchBar from '../common/SearchBar';
import FilterPanel, { FilterDateRange, FilterNumber } from '../common/FilterPanel';

import './IndexManageGoodsReceipt.scss';

import { getGoodsReceiptFiltered, ApproveGoodsReceipt, RejectGoodsReceipt, CompleteGoodsReceipt, DeleteGoodsReceipt } from '../../services/apiService';
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
    const [showModalViewGoodsReceipt, setShowModalViewGoodsReceipt] = useState(false);
    const [dataView, setDataView] = useState({});

    const [actionModal, setActionModal] = useState({ show: false, mode: '', record: {} });

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

    const handleViewGoodsReceipt = (gr) => {
        setShowModalViewGoodsReceipt(true);
        setDataView(gr);
    };

    const handleApproveGoodsReceipt = (gr) => {
        setActionModal({ show: true, mode: 'approve', record: gr });
    };

    const handleRejectGoodsReceipt = (gr) => {
        setActionModal({ show: true, mode: 'reject', record: gr });
    };

    const handleCompleteGoodsReceipt = (gr) => {
        setActionModal({ show: true, mode: 'complete', record: gr });
    };

    const handleDeleteGoodsReceipt = (gr) => {
        setActionModal({ show: true, mode: 'delete', record: gr });
    };

    const confirmAction = async (record) => {
        const { mode } = actionModal;
        let call;
        if (mode === 'approve') call = ApproveGoodsReceipt(record.goodsReceiptID);
        else if (mode === 'reject') call = RejectGoodsReceipt(record.goodsReceiptID);
        else if (mode === 'complete') call = CompleteGoodsReceipt(record.goodsReceiptID);
        else if (mode === 'delete') call = DeleteGoodsReceipt(record.goodsReceiptID);
        const res = await call;
        if (!res || res.ec !== 0) {
            throw new Error(res?.em || 'Thao tác thất bại');
        }
        await fetchListGoodsReceipt(filters);
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
                        handleViewGoodsReceipt={handleViewGoodsReceipt}
                        handleApproveGoodsReceipt={handleApproveGoodsReceipt}
                        handleRejectGoodsReceipt={handleRejectGoodsReceipt}
                        handleCompleteGoodsReceipt={handleCompleteGoodsReceipt}
                        handleUpdateGoodsReceipt={(gr) => { setShowModalUpdateGoodsReceipt(true); setDataUpdate(gr); }}
                        handleDeleteGoodsReceipt={handleDeleteGoodsReceipt}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />
                </div>

                <ModalCreateGoodsReceipt show={showModalCreateGoodsReceipt} setShow={setShowModalCreateGoodsReceipt} fetchListGoodsReceipt={() => fetchListGoodsReceipt(filters)} />
                <ModalUpdateGoodsReceipt show={showModalUpdateGoodsReceipt} setShow={setShowModalUpdateGoodsReceipt} fetchListGoodsReceipt={() => fetchListGoodsReceipt(filters)} dataUpdate={dataUpdate} />
                <ModalViewGoodsReceipt show={showModalViewGoodsReceipt} setShow={setShowModalViewGoodsReceipt} dataView={dataView} />
                <StatusActionModal
                    show={actionModal.show}
                    setShow={(v) => setActionModal(prev => ({ ...prev, show: v }))}
                    mode={actionModal.mode}
                    entityName="phiếu nhập kho"
                    record={actionModal.record}
                    onConfirm={confirmAction}
                />
            </div>
        </div>
    );
}
export default IndexManageGoodsReceipt;
