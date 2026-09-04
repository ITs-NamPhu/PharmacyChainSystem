import React from 'react';
import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

import { MdAddCircle } from "react-icons/md";
import Dropdown from 'react-bootstrap/Dropdown';

import TableStockTake from './TableStockTake';
import ModalCreateStockTake from './ModalCreateStockTake';
import ModalViewStockTake from './ModalViewStockTake';
import ModalCompleteStockTake from './ModalCompleteStockTake';
import ModalUpdateStockTake from './ModalUpdateStockTake';
import StatusActionModal from '../../common/StatusActionModal';

import './IndexManageStockTake.scss'

import { getAllStockTakePag, getWarehouseByBranch, ApproveStockTake, RejectStockTake, DeleteStockTake } from '../../../services/apiService';

const IndexStockTake = (props) => {

    const currentBranchId = useSelector(state => state.user.account.currentBranchId)

    const [warehouseID, setWarehouseID] = useState(0);
    const [warehouseList, setWarehouseList] = useState([]);

    const [stockTakeList, setStockTakeList] = useState([]);

    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateStockTake, setShowModalCreateStockTake] = useState(false);

    const [showModalViewStockTake, setShowModalViewStockTake] = useState(false);
    const [dataView, setDataView] = useState({});

    const [showModalCompleteStockTake, setShowModalCompleteStockTake] = useState(false);
    const [dataComplete, setDataComplete] = useState({});

    const [showModalUpdateStockTake, setShowModalUpdateStockTake] = useState(false);
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
        fetchStockTake(1, wId);
    }

    const fetchStockTake = async (page, warehouseIdOverride) => {
        let res = await getAllStockTakePag(page, limitPage, warehouseIdOverride ?? warehouseID);

        if (res && res.ec === 0 && res.dt) {
            setStockTakeList(res.dt.stockTakes || []);
            setPageCount(res.dt.totalPage || 0);
        }
    }

    const handleCreateStockTake = () => {
        setShowModalCreateStockTake(!showModalCreateStockTake);
    }
    const handleViewStockTake = (stockTake) => {
        setShowModalViewStockTake(!showModalViewStockTake);
        setDataView(stockTake);
    }
    const handleCompleteStockTake = (stockTake) => {
        setShowModalCompleteStockTake(!showModalCompleteStockTake);
        setDataComplete(stockTake);
    }
    const handleUpdateStockTake = (stockTake) => {
        setShowModalUpdateStockTake(!showModalUpdateStockTake);
        setDataUpdate(stockTake);
    }
    const handleApproveStockTake = (stockTake) => {
        setActionModal({ show: true, mode: 'approve', record: stockTake });
    }
    const handleRejectStockTake = (stockTake) => {
        setActionModal({ show: true, mode: 'reject', record: stockTake });
    }
    const handleDeleteStockTake = (stockTake) => {
        setActionModal({ show: true, mode: 'delete', record: stockTake });
    }

    const confirmAction = async (record) => {
        const { mode } = actionModal;
        let call;
        if (mode === 'approve') call = ApproveStockTake(record.stockTakeID);
        else if (mode === 'reject') call = RejectStockTake(record.stockTakeID);
        else if (mode === 'delete') call = DeleteStockTake(record.stockTakeID);
        const res = await call;
        if (!res || res.ec !== 0) {
            throw new Error(res?.em || 'Thao tác thất bại');
        }
        await fetchStockTake(1, warehouseID);
    }

    const getWarehouseNameById = (id) => {
        const warehouse = warehouseList.find(wh => wh.warehouseID === id);
        return warehouse ? warehouse.warehouseName : '';
    }

    return (
        <div className='manage-stockTake-container'>
            <div className="title">
                Management Stock Take
            </div>
            <div className="stockTake-content">
                <div className='btn-add-new'>
                    <Dropdown>
                        <Dropdown.Toggle variant="success" id="dropdown-basic">
                            {getWarehouseNameById(warehouseID) || 'Select Warehouse'}
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            {warehouseList.map((warehouse) => (
                                <Dropdown.Item
                                    key={warehouse.warehouseID}
                                    onClick={() => { setWarehouseID(warehouse.warehouseID); fetchStockTake(1, warehouse.warehouseID); }}
                                >
                                    {warehouse.warehouseName}
                                </Dropdown.Item>
                            ))}
                        </Dropdown.Menu>
                    </Dropdown>

                    <button className='btn btn-primary' onClick={() => handleCreateStockTake()}>
                        <MdAddCircle /> Add Stock Take
                    </button>
                </div >
                <div className='table-stockTake-container'>
                    <TableStockTake
                        fetchStockTake={fetchStockTake}

                        listStockTake={stockTakeList}

                        handleViewStockTake={handleViewStockTake}
                        handleCompleteStockTake={handleCompleteStockTake}
                        handleUpdateStockTake={handleUpdateStockTake}
                        handleApproveStockTake={handleApproveStockTake}
                        handleRejectStockTake={handleRejectStockTake}
                        handleDeleteStockTake={handleDeleteStockTake}

                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />

                    <ModalCreateStockTake
                        show={showModalCreateStockTake} setShow={setShowModalCreateStockTake}
                        fetchStockTake={fetchStockTake}
                        listWarehouse={warehouseList}
                    />

                    <ModalViewStockTake
                        show={showModalViewStockTake} setShow={setShowModalViewStockTake}
                        dataView={dataView}
                    />

                    <ModalCompleteStockTake
                        show={showModalCompleteStockTake} setShow={setShowModalCompleteStockTake}
                        fetchStockTake={fetchStockTake}
                        dataComplete={dataComplete}
                    />

                    <ModalUpdateStockTake
                        show={showModalUpdateStockTake} setShow={setShowModalUpdateStockTake}
                        fetchStockTake={fetchStockTake}
                        listWarehouse={warehouseList}
                        dataUpdate={dataUpdate}
                    />

                    <StatusActionModal
                        show={actionModal.show}
                        setShow={(v) => setActionModal(prev => ({ ...prev, show: v }))}
                        mode={actionModal.mode}
                        entityName="phiếu kiểm kê"
                        record={actionModal.record}
                        onConfirm={confirmAction}
                    />
                </div>
            </div>
        </div>
    );
}

export default IndexStockTake;
