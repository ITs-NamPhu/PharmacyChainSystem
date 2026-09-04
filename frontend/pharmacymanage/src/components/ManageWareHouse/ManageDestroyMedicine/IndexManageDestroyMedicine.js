import React from 'react';
import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

import { MdAddCircle } from "react-icons/md";
import Dropdown from 'react-bootstrap/Dropdown';

import TableDestroyReceipt from './TableDestroyReceipt';
import ModalCreateDestroyReceipt from './ModalCreateDestroyReceipt';
import ModalViewDestroyReceipt from './ModalViewDestroyReceipt';
import ModalCompleteDestroyReceipt from './ModalCompleteDestroyReceipt';
import ModalUpdateDestroyReceipt from './ModalUpdateDestroyReceipt';
import StatusActionModal from '../../common/StatusActionModal';

import './IndexManageDestroyMedicine.scss'

import { getAllDestroyReceiptPag, getWarehouseByBranch, ApproveDestroyReceipt, RejectDestroyReceipt, DeleteDestroyReceipt } from '../../../services/apiService';

const IndexManageDestroyMedicine = (props) => {

    const currentBranchId = useSelector(state => state.user.account.currentBranchId)

    const [warehouseID, setWarehouseID] = useState(0);
    const [warehouseList, setWarehouseList] = useState([]);

    const [destroyReceiptList, setDestroyReceiptList] = useState([]);

    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateDestroyReceipt, setShowModalCreateDestroyReceipt] = useState(false);

    const [showModalViewDestroyReceipt, setShowModalViewDestroyReceipt] = useState(false);
    const [dataView, setDataView] = useState({});

    const [showModalCompleteDestroyReceipt, setShowModalCompleteDestroyReceipt] = useState(false);
    const [dataComplete, setDataComplete] = useState({});

    const [showModalUpdateDestroyReceipt, setShowModalUpdateDestroyReceipt] = useState(false);
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
        fetchDestroyReceipt(1, wId);
    }

    const fetchDestroyReceipt = async (page, warehouseIdOverride) => {
        let res = await getAllDestroyReceiptPag(page, limitPage, warehouseIdOverride ?? warehouseID);

        if (res && res.ec === 0 && res.dt) {
            setDestroyReceiptList(res.dt.destroyReceipts || []);
            setPageCount(res.dt.totalPage || 0);
        }
    }

    const handleCreateDestroyReceipt = () => {
        setShowModalCreateDestroyReceipt(!showModalCreateDestroyReceipt);
    }
    const handleViewDestroyReceipt = (destroyReceipt) => {
        setShowModalViewDestroyReceipt(!showModalViewDestroyReceipt);
        setDataView(destroyReceipt);
    }
    const handleCompleteDestroyReceipt = (destroyReceipt) => {
        setShowModalCompleteDestroyReceipt(!showModalCompleteDestroyReceipt);
        setDataComplete(destroyReceipt);
    }
    const handleUpdateDestroyReceipt = (destroyReceipt) => {
        setShowModalUpdateDestroyReceipt(!showModalUpdateDestroyReceipt);
        setDataUpdate(destroyReceipt);
    }
    const handleApproveDestroyReceipt = (destroyReceipt) => {
        setActionModal({ show: true, mode: 'approve', record: destroyReceipt });
    }
    const handleRejectDestroyReceipt = (destroyReceipt) => {
        setActionModal({ show: true, mode: 'reject', record: destroyReceipt });
    }
    const handleDeleteDestroyReceipt = (destroyReceipt) => {
        setActionModal({ show: true, mode: 'delete', record: destroyReceipt });
    }

    const confirmAction = async (record) => {
        const { mode } = actionModal;
        let call;
        if (mode === 'approve') call = ApproveDestroyReceipt(record.destroyReceiptID);
        else if (mode === 'reject') call = RejectDestroyReceipt(record.destroyReceiptID);
        else if (mode === 'delete') call = DeleteDestroyReceipt(record.destroyReceiptID);
        const res = await call;
        if (!res || res.ec !== 0) {
            throw new Error(res?.em || 'Thao tác thất bại');
        }
        await fetchDestroyReceipt(1, warehouseID);
    }

    const getWarehouseNameById = (id) => {
        const warehouse = warehouseList.find(wh => wh.warehouseID === id);
        return warehouse ? warehouse.warehouseName : '';
    }

    return (
        <div className='manage-destroy-receipt-container'>
            <div className="title">
                Management Destroy Medicine
            </div>
            <div className="destroy-receipt-content">
                <div className='btn-add-new'>
                    <Dropdown>
                        <Dropdown.Toggle variant="success" id="dropdown-basic">
                            {getWarehouseNameById(warehouseID) || 'Select Warehouse'}
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            {warehouseList.map((warehouse) => (
                                <Dropdown.Item
                                    key={warehouse.warehouseID}
                                    onClick={() => { setWarehouseID(warehouse.warehouseID); fetchDestroyReceipt(1, warehouse.warehouseID); }}
                                >
                                    {warehouse.warehouseName}
                                </Dropdown.Item>
                            ))}
                        </Dropdown.Menu>
                    </Dropdown>

                    <button className='btn btn-primary' onClick={() => handleCreateDestroyReceipt()}>
                        <MdAddCircle /> Add Destroy Receipt
                    </button>
                </div >
                <div className='table-destroy-receipt-container'>
                    <TableDestroyReceipt
                        fetchDestroyReceipt={fetchDestroyReceipt}

                        listDestroyReceipt={destroyReceiptList}

                        handleViewDestroyReceipt={handleViewDestroyReceipt}
                        handleCompleteDestroyReceipt={handleCompleteDestroyReceipt}
                        handleUpdateDestroyReceipt={handleUpdateDestroyReceipt}
                        handleApproveDestroyReceipt={handleApproveDestroyReceipt}
                        handleRejectDestroyReceipt={handleRejectDestroyReceipt}
                        handleDeleteDestroyReceipt={handleDeleteDestroyReceipt}

                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    />

                    <ModalCreateDestroyReceipt
                        show={showModalCreateDestroyReceipt} setShow={setShowModalCreateDestroyReceipt}
                        fetchDestroyReceipt={fetchDestroyReceipt}
                        listWarehouse={warehouseList}
                        warehouseID={warehouseID}
                    />

                    <ModalViewDestroyReceipt
                        show={showModalViewDestroyReceipt} setShow={setShowModalViewDestroyReceipt}
                        dataView={dataView}
                    />

                    <ModalCompleteDestroyReceipt
                        show={showModalCompleteDestroyReceipt} setShow={setShowModalCompleteDestroyReceipt}
                        fetchDestroyReceipt={fetchDestroyReceipt}
                        dataComplete={dataComplete}
                    />

                    <ModalUpdateDestroyReceipt
                        show={showModalUpdateDestroyReceipt} setShow={setShowModalUpdateDestroyReceipt}
                        fetchDestroyReceipt={fetchDestroyReceipt}
                        dataUpdate={dataUpdate}
                    />

                    <StatusActionModal
                        show={actionModal.show}
                        setShow={(v) => setActionModal(prev => ({ ...prev, show: v }))}
                        mode={actionModal.mode}
                        entityName="phiếu tiêu hủy"
                        record={actionModal.record}
                        onConfirm={confirmAction}
                    />
                </div>
            </div>
        </div>
    );
}

export default IndexManageDestroyMedicine;
