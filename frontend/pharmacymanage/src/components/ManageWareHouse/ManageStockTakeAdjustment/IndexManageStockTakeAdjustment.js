import React from 'react';
import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

import { MdAddCircle } from "react-icons/md";
import Dropdown from 'react-bootstrap/Dropdown';

import TableStockAdjustment from './TableStockAdjustment';
import ModalCreateStockAdjustment from './ModalCreateStockAdjustment';
import ModalViewStockAdjustment from './ModalViewStockAdjustment';
import ModalUpdateStockAdjustment from './ModalUpdateStockAdjustment';
import StatusActionModal from '../../common/StatusActionModal';

import './IndexManageStockTakeAdjustment.scss'

import { getAllStockAdjustmentPag, getWarehouseByBranch, ApproveStockAdjustment, RejectStockAdjustment, DeleteStockAdjustment, CompleteStockAdjustment } from '../../../services/apiService';

const IndexStockTakeAdjustment = (props) => {

    const currentBranchId = useSelector(state => state.user.account.currentBranchId)

    const [warehouseID, setWarehouseID] = useState(0);
    const [warehouseList, setWarehouseList] = useState([]);

    const [stockAdjustmentList, setStockAdjustmentList] = useState([]);

    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateStockAdjustment, setShowModalCreateStockAdjustment] = useState(false);

    const [showModalViewStockAdjustment, setShowModalViewStockAdjustment] = useState(false);
    const [dataView, setDataView] = useState({});

    const [showModalUpdateStockAdjustment, setShowModalUpdateStockAdjustment] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [actionModal, setActionModal] = useState({ show: false, mode: '', record: {} });

    useEffect(() => {
        fetchWarehouse();
    }, [currentBranchId])

    const fetchWarehouse = async () => {
        let wId = 0;

        let res = await getWarehouseByBranch(currentBranchId, 1, 10);

        if (res && res.ec === 0 && res.dt) {
            const warehouses = res.dt.warehouses || [];
            setWarehouseList(warehouses);

            wId = warehouses.length > 0 ? warehouses[0].warehouseID : 0;
        }

        setWarehouseID(wId);
        fetchStockAdjustment(1, wId);
    }

    const fetchStockAdjustment = async (page, warehouseIdOverride) => {
        let res = await getAllStockAdjustmentPag(page, limitPage, warehouseIdOverride ?? warehouseID);

        if (res && res.ec === 0 && res.dt) {
            setStockAdjustmentList(res.dt.stockAdjustments || []);
            setPageCount(res.dt.totalPage || 0);
        }
    }

    const handleCreateStockAdjustment = () => {
        setShowModalCreateStockAdjustment(!showModalCreateStockAdjustment);
    }
    const handleViewStockAdjustment = (stockAdjustment) => {
        setShowModalViewStockAdjustment(!showModalViewStockAdjustment);
        setDataView(stockAdjustment);
    }
    const handleUpdateStockAdjustment = (stockAdjustment) => {
        setShowModalUpdateStockAdjustment(!showModalUpdateStockAdjustment);
        setDataUpdate(stockAdjustment);
    }
    const handleApproveStockAdjustment = (stockAdjustment) => {
        setActionModal({ show: true, mode: 'approve', record: stockAdjustment });
    }
    const handleRejectStockAdjustment = (stockAdjustment) => {
        setActionModal({ show: true, mode: 'reject', record: stockAdjustment });
    }
    const handleCompleteStockAdjustment = (stockAdjustment) => {
        setActionModal({ show: true, mode: 'complete', record: stockAdjustment });
    }
    const handleDeleteStockAdjustment = (stockAdjustment) => {
        setActionModal({ show: true, mode: 'delete', record: stockAdjustment });
    }

    const confirmAction = async (record) => {
        const { mode } = actionModal;
        let call;
        if (mode === 'approve') call = ApproveStockAdjustment(record.stockAdjustmentID);
        else if (mode === 'reject') call = RejectStockAdjustment(record.stockAdjustmentID);
        else if (mode === 'complete') call = CompleteStockAdjustment(record.stockAdjustmentID);
        else if (mode === 'delete') call = DeleteStockAdjustment(record.stockAdjustmentID);
        const res = await call;
        if (!res || res.ec !== 0) {
            throw new Error(res?.em || 'Thao tác thất bại');
        }
        await fetchStockAdjustment(1, warehouseID);
    }

    const getWarehouseNameById = (id) => {
        const warehouse = warehouseList.find(wh => wh.warehouseID === id);
        return warehouse ? warehouse.warehouseName : '';
    }

    return (
        <div className='manage-stockTake-adjustment-container'>
            <div className="title">
                Management Stock Adjustment
            </div>
            <div className="stockTake-adjustment-content">
                <div className='btn-add-new'>
                    <Dropdown>
                        <Dropdown.Toggle variant="success" id="dropdown-basic">
                            {getWarehouseNameById(warehouseID) || 'Select Warehouse'}
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            {warehouseList.map((warehouse) => (
                                <Dropdown.Item
                                    key={warehouse.warehouseID}
                                    onClick={() => { setWarehouseID(warehouse.warehouseID); fetchStockAdjustment(1, warehouse.warehouseID); }}
                                >
                                    {warehouse.warehouseName}
                                </Dropdown.Item>
                            ))}
                        </Dropdown.Menu>
                    </Dropdown>

                    <button className='btn btn-primary' onClick={() => handleCreateStockAdjustment()}>
                        <MdAddCircle /> Add Stock Adjustment
                    </button>
                </div >
                <div className='table-stockTake-adjustment-container'>
                    <TableStockAdjustment
                        fetchStockAdjustment={fetchStockAdjustment}

                        listStockAdjustment={stockAdjustmentList}

                        handleViewStockAdjustment={handleViewStockAdjustment}
                        handleApproveStockAdjustment={handleApproveStockAdjustment}
                        handleRejectStockAdjustment={handleRejectStockAdjustment}
                        handleCompleteStockAdjustment={handleCompleteStockAdjustment}
                        handleUpdateStockAdjustment={handleUpdateStockAdjustment}
                        handleDeleteStockAdjustment={handleDeleteStockAdjustment}

                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />

                    <ModalCreateStockAdjustment
                        show={showModalCreateStockAdjustment} setShow={setShowModalCreateStockAdjustment}
                        fetchStockAdjustment={fetchStockAdjustment}
                        listWarehouse={warehouseList}
                        warehouseID={warehouseID}
                    />

                    <ModalViewStockAdjustment
                        show={showModalViewStockAdjustment} setShow={setShowModalViewStockAdjustment}
                        dataView={dataView}
                    />

                    <ModalUpdateStockAdjustment
                        show={showModalUpdateStockAdjustment} setShow={setShowModalUpdateStockAdjustment}
                        fetchStockAdjustment={fetchStockAdjustment}
                        dataUpdate={dataUpdate}
                    />

                    <StatusActionModal
                        show={actionModal.show}
                        setShow={(v) => setActionModal(prev => ({ ...prev, show: v }))}
                        mode={actionModal.mode}
                        entityName="phiếu điều chỉnh"
                        record={actionModal.record}
                        onConfirm={confirmAction}
                    />
                </div>
            </div>
        </div>
    );
}

export default IndexStockTakeAdjustment;
