import { useState, useEffect } from 'react';

import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Tabs from 'react-bootstrap/Tabs';
import Tab from 'react-bootstrap/Tab';

import { FaPlus, FaTrash } from 'react-icons/fa';
import { toast } from 'react-toastify';

import { useSelector } from 'react-redux';

import {
    UpdateInvoice,
    GetInvoiceById,

    getAllCustomerNoPag,
    getAllMedicineNoPag,
    GetUsersByBranch,
    GetAllUnitNoPag,

    GetBatchesByMedicine
} from '../../services/apiService';

const ModalUpdateInvoiceLot = (props) => {
    const { show, setShow, dataUpdate } = props;

    const account = useSelector(state => state.user.account);

    const roleName = account.roleName;
    const canChangeUser = roleName === 'admin' || roleName === 'manage_supply' || roleName === 'manage_branch';

    const [activeTab, setActiveTab] = useState('invoiceInfo');

    const [customerID, setCustomerID] = useState('');
    const [note, setNote] = useState('');
    const [createdByUserID, setCreatedByUserID] = useState('');

    const [listCustomer, setListCustomer] = useState([]);
    const [listMedicine, setListMedicine] = useState([]);
    const [listUser, setListUser] = useState([]);
    const [listUnit, setListUnit] = useState([]);

    const [invoiceItems, setInvoiceItems] = useState([]);

    useEffect(() => {
        if (show && dataUpdate?.invoiceID) {
            fetchInvoiceData();
        }
    }, [show, dataUpdate]);

    const fetchInvoiceData = async () => {
        let [resInv, resCus, resMed, resUser, resUnit] = await Promise.all([
            GetInvoiceById(dataUpdate.invoiceID),
            getAllCustomerNoPag(),
            getAllMedicineNoPag(),
            GetUsersByBranch(),
            GetAllUnitNoPag()
        ]);

        if (resCus && resCus.ec === 0) setListCustomer(resCus.dt);
        if (resMed && resMed.ec === 0) setListMedicine(resMed.dt);
        if (resUser && resUser.ec === 0) setListUser(resUser.dt);
        if (resUnit && resUnit.ec === 0) setListUnit(resUnit.dt);

        if (resInv && resInv.ec === 0) {
            const inv = resInv.dt;
            setCustomerID(inv.customerID);
            setNote(inv.note || '');
            setCreatedByUserID(inv.userID);

            const items = inv.invoiceItems.map(ii => ({
                invoiceItemID: ii.invoiceItemID,
                medicineID: ii.medicineID,
                batchID: ii.batchID,
                unitID: ii.unitID,
                unitName: ii.unitName,
                quantity: ii.quantity,
                unitPrice: ii.unitPrice,
                availableBatches: []
            }));

            for (let item of items) {
                const res = await GetBatchesByMedicine(item.medicineID);
                if (res && res.ec === 0) {
                    item.availableBatches = res.dt;
                }
            }

            setInvoiceItems(items);
        }
    };

    const handleClose = () => {
        setShow(false);
        setActiveTab('invoiceInfo');
        setCustomerID('');
        setNote('');
        setCreatedByUserID('');
        setInvoiceItems([]);
    };

    const handleAddItem = () => {
        setInvoiceItems([...invoiceItems, {
            medicineID: '',
            batchID: null,
            unitID: '',
            unitName: '',
            quantity: 1,
            unitPrice: 0,
            availableBatches: []
        }]);
    };

    const handleRemoveItem = (index) => {
        const updated = [...invoiceItems];
        updated.splice(index, 1);
        setInvoiceItems(updated);
    };

    const handleMedicineChange = async (index, medicineID) => {
        const updated = [...invoiceItems];
        updated[index].medicineID = medicineID;
        updated[index].batchID = null;
        updated[index].unitName = '';
        updated[index].availableBatches = [];

        const medicine = listMedicine.find(m => m.medicineID === +medicineID);
        if (medicine) {
            updated[index].unitPrice = medicine.defaultRetailPrice;
            updated[index].unitID = medicine.baseUnitID;
        }

        if (medicineID) {
            const res = await GetBatchesByMedicine(medicineID);
            if (res && res.ec === 0) {
                updated[index].availableBatches = res.dt;
            }
        }

        setInvoiceItems(updated);
    };

    const handleQuantityChange = (index, quantity) => {
        const updated = [...invoiceItems];
        updated[index].quantity = +quantity;
        setInvoiceItems(updated);
    };

    const handleBatchChange = (index, batchID) => {
        const updated = [...invoiceItems];
        updated[index].batchID = +batchID;
        setInvoiceItems(updated);
    };

    const handleUnitChange = (index, unitID) => {
        const updated = [...invoiceItems];
        updated[index].unitID = +unitID;
        setInvoiceItems(updated);
    };

    const getSelectedBatchIDs = (medicineID, currentIndex) => {
        return invoiceItems
            .filter((item, idx) => idx !== currentIndex && item.medicineID === medicineID && item.batchID)
            .map(item => item.batchID);
    };

    const formatPrice = (price) => {
        if (price == null) return '0';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    const getTotalAmount = () => {
        return invoiceItems.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
    };

    const handleSubmitInvoice = async () => {
        if (!customerID || customerID === '') {
            toast.error('Vui lòng chọn khách hàng');
            setActiveTab('invoiceInfo');
            return;
        }

        if (invoiceItems.length === 0) {
            toast.error('Vui lòng thêm ít nhất 1 mặt hàng');
            setActiveTab('invoiceItems');
            return;
        }

        for (let i = 0; i < invoiceItems.length; i++) {
            const item = invoiceItems[i];
            if (!item.medicineID) {
                toast.error(`Vui lòng chọn thuốc cho mặt hàng thứ ${i + 1}`);
                setActiveTab('invoiceItems');
                return;
            }
            if (!item.quantity || item.quantity <= 0) {
                toast.error(`Số lượng phải lớn hơn 0 cho mặt hàng thứ ${i + 1}`);
                setActiveTab('invoiceItems');
                return;
            }
            if (!item.batchID) {
                toast.error(`Vui lòng chọn lô cho mặt hàng thứ ${i + 1}`);
                setActiveTab('invoiceItems');
                return;
            }
        }

        const submitItems = invoiceItems.map(item => ({
            MedicineID: +item.medicineID,
            BatchID: +item.batchID,
            UnitID: +item.unitID,
            Quantity: +item.quantity,
            UnitPrice: +item.unitPrice
        }));

        let res = await UpdateInvoice(
            dataUpdate.invoiceID,
            +customerID,
            note,
            canChangeUser && createdByUserID ? +createdByUserID : null,
            submitItems,
            'Manual'
        );

        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Cập nhật hóa đơn thất bại');
            return;
        }

        toast.success(res.EM || 'Cập nhật hóa đơn thành công');
        handleClose();
        await props.fetchListInvoice(1);
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-update-invoice'>
                <Modal.Header closeButton>
                    <Modal.Title>Cập nhật hóa đơn #{dataUpdate?.invoiceID}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Tabs
                        activeKey={activeTab}
                        onSelect={(k) => setActiveTab(k)}
                        className="mb-3"
                    >
                        <Tab eventKey="invoiceInfo" title="Thông tin hóa đơn">
                            <form className='row g-3'>
                                <div className="form-group col-md-6">
                                    <label>Khách hàng <span className="text-danger">*</span></label>
                                    <select className="form-control" value={customerID} onChange={(event) => setCustomerID(event.target.value)}>
                                        <option value="">-- Chọn khách hàng --</option>
                                        {listCustomer && listCustomer.length > 0 &&
                                            listCustomer.map((item, index) => (
                                                <option key={`option-cus-${index}`} value={item.customerID}>{item.customerName}</option>
                                            ))
                                        }
                                    </select>
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Người lập</label>
                                    <select
                                        className="form-control"
                                        value={createdByUserID || ''}
                                        onChange={(event) => setCreatedByUserID(event.target.value)}
                                        disabled={!canChangeUser}
                                    >
                                        {listUser && listUser.length > 0 &&
                                            listUser.map((item, index) => (
                                                <option key={`option-user-${index}`} value={item.userID}>{item.fullName}</option>
                                            ))
                                        }
                                    </select>
                                    {!canChangeUser && (
                                        <small className="text-muted">Chỉ admin và quản lý kho mới có thể thay đổi người lập</small>
                                    )}
                                </div>
                                <div className="form-group col-md-12">
                                    <label>Ghi chú</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={note}
                                        placeholder="Nhập ghi chú (không bắt buộc)"
                                        onChange={(event) => setNote(event.target.value)}
                                    />
                                </div>
                            </form>
                        </Tab>

                        <Tab eventKey="invoiceItems" title="Chi tiết hóa đơn">
                            <div className='invoice-items-tab'>
                                <div className='d-flex justify-content-between align-items-center mb-2'>
                                    <div>
                                        <span className='fw-bold'>Chế độ: </span>
                                        <span className='badge bg-info'>LOT - Chọn lô thủ công</span>
                                    </div>
                                    <button
                                        type="button"
                                        className='btn btn-success btn-sm'
                                        onClick={handleAddItem}
                                    >
                                        <FaPlus /> Thêm thuốc
                                    </button>
                                </div>

                                <table className="table table-bordered table-sm">
                                    <thead className="table-light">
                                        <tr>
                                            <th style={{ width: '20%' }}>Thuốc</th>
                                            <th style={{ width: '10%' }}>Đơn giá</th>
                                            <th style={{ width: '12%' }}>Đơn vị</th>
                                            <th style={{ width: '8%' }}>Số lượng</th>
                                            <th style={{ width: '18%' }}>Lô (Batch)</th>
                                            <th style={{ width: '15%' }}>Thành tiền</th>
                                            <th style={{ width: '8%' }}>Thao tác</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {invoiceItems.length === 0 &&
                                            <tr>
                                                <td colSpan={7} className="text-center text-muted">
                                                    Chưa có mặt hàng. Nhấn "+ Thêm thuốc" để thêm.
                                                </td>
                                            </tr>
                                        }

                                        {invoiceItems.map((item, index) => {
                                            const subTotal = (item.unitPrice || 0) * (item.quantity || 0);
                                            return (
                                                <tr key={`item-row-${index}`}>
                                                    <td>
                                                        <select
                                                            className="form-control form-control-sm"
                                                            value={item.medicineID}
                                                            onChange={(event) => handleMedicineChange(index, event.target.value)}
                                                        >
                                                            <option value="">-- Chọn thuốc --</option>
                                                            {listMedicine && listMedicine.length > 0 &&
                                                                listMedicine.map((med, idx) => (
                                                                    <option key={`med-${index}-${idx}`} value={med.medicineID}>
                                                                        {med.medicineName}
                                                                    </option>
                                                                ))
                                                            }
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="number"
                                                            className="form-control form-control-sm"
                                                            value={item.unitPrice}
                                                            min="0"
                                                            disabled
                                                        />
                                                    </td>
                                                    <td>
                                                        <select
                                                            className="form-control form-control-sm"
                                                            value={item.unitID || ''}
                                                            onChange={(event) => handleUnitChange(index, event.target.value)}
                                                            disabled={!item.medicineID}
                                                        >
                                                            <option value="">-- Chọn đơn vị --</option>
                                                            {listUnit && listUnit.length > 0 &&
                                                                listUnit.map((unit, idx) => (
                                                                    <option key={`unit-${index}-${idx}`} value={unit.unitID}>
                                                                        {unit.unitName}
                                                                    </option>
                                                                ))
                                                            }
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="number"
                                                            className="form-control form-control-sm"
                                                            value={item.quantity}
                                                            min="1"
                                                            onChange={(event) => handleQuantityChange(index, event.target.value)}
                                                        />
                                                    </td>
                                                    <td>
                                                        <select
                                                            className="form-control form-control-sm"
                                                            value={item.batchID || ''}
                                                            onChange={(event) => handleBatchChange(index, event.target.value)}
                                                            disabled={!item.medicineID}
                                                        >
                                                            <option value="">-- Chọn lô --</option>
                                                            {item.availableBatches && item.availableBatches.length > 0 &&
                                                                item.availableBatches
                                                                    .filter(batch => {
                                                                        const selectedBatchIDs = getSelectedBatchIDs(item.medicineID, index);
                                                                        return !selectedBatchIDs.includes(batch.batchID);
                                                                    })
                                                                    .map((batch, idx) => (
                                                                        <option key={`batch-${index}-${idx}`} value={batch.batchID}>
                                                                            Batch {batch.batchNumber} - SL: {batch.quantityInStock} - HSD: {new Date(batch.expiryDate).toLocaleDateString('vi-VN')}
                                                                        </option>
                                                                    ))
                                                            }
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <span className="fw-bold">{formatPrice(subTotal)} đ</span>
                                                    </td>
                                                    <td className="text-center">
                                                        <button
                                                            type="button"
                                                            className="btn btn-danger btn-sm"
                                                            onClick={() => handleRemoveItem(index)}
                                                        >
                                                            <FaTrash />
                                                        </button>
                                                    </td>
                                                </tr>
                                            );
                                        })}
                                    </tbody>
                                    {invoiceItems.length > 0 &&
                                        <tfoot>
                                            <tr className="table-info">
                                                <td colSpan={5} className="text-end fw-bold">Tổng cộng:</td>
                                                <td className="fw-bold">{formatPrice(getTotalAmount())} đ</td>
                                                <td></td>
                                            </tr>
                                        </tfoot>
                                    }
                                </table>
                            </div>
                        </Tab>
                    </Tabs>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitInvoice()}>
                        Lưu
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalUpdateInvoiceLot;
