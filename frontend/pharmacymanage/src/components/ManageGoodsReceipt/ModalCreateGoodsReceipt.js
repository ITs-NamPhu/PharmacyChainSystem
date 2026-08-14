import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Tabs from 'react-bootstrap/Tabs';
import Tab from 'react-bootstrap/Tab';
import { FaPlus, FaTrash } from 'react-icons/fa';
import { toast } from 'react-toastify';

import {
    CreateGoodsReceipt,
    GetAllUnitNoPag,
    getAllMedicineNoPag,
    getAllSupplierNoPag
} from '../../services/apiService';

const ModalCreateGoodsReceipt = (props) => {
    const { show, setShow } = props;

    const [activeTab, setActiveTab] = useState('receiptInfo');

    const [supplierID, setSupplierID] = useState('');
    const [receiptDate, setReceiptDate] = useState('');
    const [paidAmount, setPaidAmount] = useState('');
    const [note, setNote] = useState('');

    const [listSupplier, setListSupplier] = useState([]);
    const [listMedicine, setListMedicine] = useState([]);
    const [listUnit, setListUnit] = useState([]);

    const [items, setItems] = useState([]);

    useEffect(() => {
        fetchDropdownData();
    }, []);

    const fetchDropdownData = async () => {
        let [resSup, resMed, resUnit] = await Promise.all([
            getAllSupplierNoPag(),
            getAllMedicineNoPag(),
            GetAllUnitNoPag()
        ]);

        if (resSup && resSup.ec === 0) {
            setListSupplier(resSup.dt);
        }
        if (resMed && resMed.ec === 0) {
            setListMedicine(resMed.dt);
        }
        if (resUnit && resUnit.ec === 0) {
            setListUnit(resUnit.dt);
        }
    };

    const handleClose = () => {
        setShow(false);
        setActiveTab('receiptInfo');
        setSupplierID('');
        setReceiptDate('');
        setPaidAmount('');
        setNote('');
        setItems([]);
    };

    const handleAddItem = () => {
        setItems([...items, { medicineID: '', unitID: '', quantity: '', unitCost: '', manufactureDate: '', expiryDate: '' }]);
    };

    const handleRemoveItem = (index) => {
        const updated = [...items];
        updated.splice(index, 1);
        setItems(updated);
    };

    const handleChangeItem = (index, field, value) => {
        const updated = [...items];
        updated[index][field] = value;
        setItems(updated);
    };

    const getMedicineName = (medicineID) => {
        const found = listMedicine.find(m => m.medicineID === +medicineID);
        return found ? found.medicineName : '';
    };

    const getUnitName = (unitID) => {
        const found = listUnit.find(u => u.unitID === +unitID);
        return found ? found.unitName : '';
    };

    const calculateTotalAmount = () => {
        return items.reduce((sum, item) => {
            const qty = parseFloat(item.quantity) || 0;
            const cost = parseFloat(item.unitCost) || 0;
            return sum + (qty * cost);
        }, 0);
    };

    const handleSubmitGoodsReceipt = async () => {
        if (!supplierID || supplierID === '') {
            toast.error('Vui lòng chọn nhà cung cấp');
            setActiveTab('receiptInfo');
            return;
        }
        if (!receiptDate || receiptDate === '') {
            toast.error('Vui lòng chọn ngày nhập');
            setActiveTab('receiptInfo');
            return;
        }
        if (items.length === 0) {
            toast.error('Vui lòng thêm ít nhất 1 chi tiết');
            setActiveTab('receiptDetails');
            return;
        }

        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            if (!item.medicineID || item.medicineID === '') {
                toast.error(`Vui lòng chọn thuốc cho dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (!item.unitID || item.unitID === '') {
                toast.error(`Vui lòng chọn đơn vị cho dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (!item.quantity || +item.quantity <= 0) {
                toast.error(`Số lượng phải lớn hơn 0 tại dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (!item.unitCost || +item.unitCost <= 0) {
                toast.error(`Đơn giá phải lớn hơn 0 tại dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (!item.manufactureDate) {
                toast.error(`Vui lòng chọn ngày sản xuất tại dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (!item.expiryDate) {
                toast.error(`Vui lòng chọn hạn sử dụng tại dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
            if (new Date(item.expiryDate) <= new Date(item.manufactureDate)) {
                toast.error(`Hạn sử dụng phải sau ngày sản xuất tại dòng ${i + 1}`);
                setActiveTab('receiptDetails');
                return;
            }
        }

        const submitItems = items.map(item => ({
            MedicineID: +item.medicineID,
            UnitID: +item.unitID,
            Quantity: +item.quantity,
            UnitCost: +item.unitCost,
            ManufactureDate: item.manufactureDate,
            ExpiryDate: item.expiryDate
        }));

        let res = await CreateGoodsReceipt(
            +supplierID,
            note,
            +paidAmount || 0,
            receiptDate,
            submitItems
        );

        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Tạo phiếu nhập kho thất bại');
            return;
        }

        toast.success(res.EM || 'Tạo phiếu nhập kho thành công');
        handleClose();
        await props.fetchListGoodsReceipt(1);
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-goods-receipt'>
                <Modal.Header closeButton>
                    <Modal.Title>Thêm phiếu nhập kho mới</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Tabs
                        activeKey={activeTab}
                        onSelect={(k) => setActiveTab(k)}
                        className="mb-3"
                    >
                        <Tab eventKey="receiptInfo" title="Thông tin phiếu nhập">
                            <form className='row g-3'>
                                <div className="form-group col-md-6">
                                    <label>Nhà cung cấp <span className="text-danger">*</span></label>
                                    <select className="form-control" value={supplierID} onChange={(event) => setSupplierID(event.target.value)}>
                                        <option value="">-- Chọn nhà cung cấp --</option>
                                        {listSupplier && listSupplier.length > 0 &&
                                            listSupplier.map((item, index) => (
                                                <option key={`option-sup-${index}`} value={item.supplierID}>{item.supplierName}</option>
                                            ))
                                        }
                                    </select>
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Ngày nhập <span className="text-danger">*</span></label>
                                    <input
                                        type="date"
                                        className="form-control"
                                        value={receiptDate}
                                        onChange={(event) => setReceiptDate(event.target.value)}
                                    />
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Số tiền đã trả (đ)</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={paidAmount}
                                        placeholder="0"
                                        min="0"
                                        onChange={(event) => setPaidAmount(event.target.value)}
                                    />
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Ghi chú</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={note}
                                        placeholder="Ghi chú"
                                        onChange={(event) => setNote(event.target.value)}
                                    />
                                </div>
                            </form>
                        </Tab>

                        <Tab eventKey="receiptDetails" title="Chi tiết nhập kho">
                            <div className='receipt-details-tab'>
                                <div className='d-flex justify-content-between align-items-center mb-2'>
                                    <label className='fw-bold mb-0'>Danh sách chi tiết</label>
                                    <button
                                        type="button"
                                        className='btn btn-success btn-sm'
                                        onClick={handleAddItem}
                                    >
                                        <FaPlus /> Thêm chi tiết
                                    </button>
                                </div>

                                <table className="table table-bordered table-sm">
                                    <thead className="table-light">
                                        <tr>
                                            <th style={{ width: '20%' }}>Thuốc</th>
                                            <th style={{ width: '12%' }}>Đơn vị</th>
                                            <th style={{ width: '10%' }}>Số lượng</th>
                                            <th style={{ width: '13%' }}>Đơn giá</th>
                                            <th style={{ width: '13%' }}>Thành tiền</th>
                                            <th style={{ width: '12%' }}>Ngày SX</th>
                                            <th style={{ width: '12%' }}>Hạn SD</th>
                                            <th style={{ width: '8%' }}>Thao tác</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {items.length === 0 &&
                                            <tr>
                                                <td colSpan={8} className="text-center text-muted">
                                                    Chưa có chi tiết. Nhấn "+ Thêm chi tiết" để thêm.
                                                </td>
                                            </tr>
                                        }

                                        {items.map((item, index) => {
                                            const qty = parseFloat(item.quantity) || 0;
                                            const cost = parseFloat(item.unitCost) || 0;
                                            const subTotal = qty * cost;
                                            return (
                                                <tr key={`item-row-${index}`}>
                                                    <td>
                                                        <select
                                                            className="form-control form-control-sm"
                                                            value={item.medicineID}
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'medicineID', event.target.value)
                                                            }
                                                        >
                                                            <option value="">-- Chọn thuốc --</option>
                                                            {listMedicine.map((med, idx) => (
                                                                <option key={`med-${index}-${idx}`} value={med.medicineID}>
                                                                    {med.medicineName}
                                                                </option>
                                                            ))}
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <select
                                                            className="form-control form-control-sm"
                                                            value={item.unitID}
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'unitID', event.target.value)
                                                            }
                                                        >
                                                            <option value="">-- Chọn đơn vị --</option>
                                                            {listUnit.map((unit, idx) => (
                                                                <option key={`unit-${index}-${idx}`} value={unit.unitID}>
                                                                    {unit.unitName}
                                                                </option>
                                                            ))}
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="number"
                                                            className="form-control form-control-sm"
                                                            value={item.quantity}
                                                            min="1"
                                                            placeholder="0"
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'quantity', event.target.value)
                                                            }
                                                        />
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="number"
                                                            className="form-control form-control-sm"
                                                            value={item.unitCost}
                                                            min="0"
                                                            placeholder="0"
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'unitCost', event.target.value)
                                                            }
                                                        />
                                                    </td>
                                                    <td>
                                                        <span className="fw-bold">
                                                            {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(subTotal)}
                                                        </span>
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="date"
                                                            className="form-control form-control-sm"
                                                            value={item.manufactureDate}
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'manufactureDate', event.target.value)
                                                            }
                                                        />
                                                    </td>
                                                    <td>
                                                        <input
                                                            type="date"
                                                            className="form-control form-control-sm"
                                                            value={item.expiryDate}
                                                            min={item.manufactureDate || undefined}
                                                            onChange={(event) =>
                                                                handleChangeItem(index, 'expiryDate', event.target.value)
                                                            }
                                                        />
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
                                </table>

                                {items.length > 0 && (
                                    <div className="text-end fw-bold">
                                        Tổng cộng: {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(calculateTotalAmount())}
                                    </div>
                                )}
                            </div>
                        </Tab>
                    </Tabs>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitGoodsReceipt()}>
                        Lưu
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalCreateGoodsReceipt;
