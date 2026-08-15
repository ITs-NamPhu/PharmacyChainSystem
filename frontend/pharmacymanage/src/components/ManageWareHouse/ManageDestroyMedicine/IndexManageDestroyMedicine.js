import React from 'react';
import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

import { MdAddCircle } from "react-icons/md";
import Dropdown from 'react-bootstrap/Dropdown';

import TableDestroyReceipt from './TableDestroyReceipt';
import ModalCreateDestroyReceipt from './ModalCreateDestroyReceipt';
import ModalViewDestroyReceipt from './ModalViewDestroyReceipt';
import ModalApproveDestroyReceipt from './ModalApproveDestroyReceipt';

import './IndexManageDestroyMedicine.scss'

import { getAllDestroyReceiptPag, getWarehouseByBranch } from '../../../services/apiService';

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

    const [showModalApproveDestroyReceipt, setShowModalApproveDestroyReceipt] = useState(false);
    const [dataApprove, setDataApprove] = useState({});

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
    const handleApproveDestroyReceipt = (destroyReceipt) => {
        setShowModalApproveDestroyReceipt(!showModalApproveDestroyReceipt);
        setDataApprove(destroyReceipt);
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
                                    onClick={() => setWarehouseID(warehouse.warehouseID)}
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
                        handleApproveDestroyReceipt={handleApproveDestroyReceipt}

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

                    <ModalApproveDestroyReceipt
                        show={showModalApproveDestroyReceipt} setShow={setShowModalApproveDestroyReceipt}
                        fetchDestroyReceipt={fetchDestroyReceipt}
                        dataApprove={dataApprove}
                    />
                </div>
            </div>
        </div>
    );
}

export default IndexManageDestroyMedicine;
