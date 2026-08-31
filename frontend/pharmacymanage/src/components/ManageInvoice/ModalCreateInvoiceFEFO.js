import { useState, useEffect } from 'react';

import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Tabs from 'react-bootstrap/Tabs';
import Tab from 'react-bootstrap/Tab';

import { FaPlus, FaTrash } from 'react-icons/fa';
import { toast } from 'react-toastify';

import { useSelector } from 'react-redux';

import {
    CreateInvoice,

    getAllCustomerNoPag,
    getAllMedicineNoPag,
    GetUsersByBranch,
    GetAllUnitNoPag,

    GetFefoBatches
} from '../../services/apiService';

const ModalCreateInvoiceFEFO = (props) => {
    const { show, setShow } = props;

    const account = useSelector(state => state.user.account);

    const roleName = account.roleName;
    const canChangeUser = roleName === 'admin' || roleName === 'manage_supply' || roleName === 'manage_branch';

    const [activeTab, setActiveTab] = useState('invoiceInfo');

    // state for invoice info
    const [customerID, setCustomerID] = useState('');
    const [note, setNote] = useState('');
    const [createdByUserID, setCreatedByUserID] = useState('');

    // state thanh toán
    const [customerWalletBalance, setCustomerWalletBalance] = useState(0);
    const [useWallet, setUseWallet] = useState(false);
    const [paidAmount, setPaidAmount] = useState(0);

    // state for dropdowns
    const [listCustomer, setListCustomer] = useState([]);
    const [listMedicine, setListMedicine] = useState([]);
    const [listUser, setListUser] = useState([]);
    const [listUnit, setListUnit] = useState([]);

    // state for invoice items
    const [invoiceItems, setInvoiceItems] = useState([]);

    // received FEFO results
    const [fefoResults, setFefoResults] = useState([]);

    useEffect(() => {
        fetchDropdownData();
    }, []);

    //fetch data for dropdowns
    const fetchDropdownData = async () => {
        let [resCus, resMed, resUser, resUnit] = await Promise.all([
            getAllCustomerNoPag(),
            getAllMedicineNoPag(),
            GetUsersByBranch(),
            GetAllUnitNoPag()
        ]);

        if (resCus && resCus.ec === 0) {
            setListCustomer(resCus.dt);
        }
        if (resMed && resMed.ec === 0) {
            setListMedicine(resMed.dt);
        }
        if (resUser && resUser.ec === 0) {
            setListUser(resUser.dt);
        }
        if (resUnit && resUnit.ec === 0) {
            setListUnit(resUnit.dt);
        }
    };

    const handleClose = () => {
        setShow(false);
        setActiveTab('invoiceInfo');
        setCustomerID('');
        setNote('');
        setCreatedByUserID('');
        setInvoiceItems([]);
        setFefoResults([]);
        setCustomerWalletBalance(0);
        setUseWallet(false);
        setPaidAmount(0);
    };

    // handle khi thay đổi khách hàng -> cập nhật số dư ví để hiển thị badge + thanh toán
    const handleCustomerChange = (value) => {
        setCustomerID(value);
        const customer = listCustomer.find(c => c.customerID === +value);
        setCustomerWalletBalance(customer ? +(customer.walletBalance || 0) : 0);
        setUseWallet(false);
    };

    const handleAddItem = () => {
        setInvoiceItems([...invoiceItems, {
            medicineID: '',
            batchID: null,
            unitID: '',
            unitName: '',
            quantity: 1,
            unitPrice: 0,
            fefoDisplay: '',
            availableBatches: []
        }]);
    };

    const handleRemoveItem = (index) => {
        const updated = [...invoiceItems];
        updated.splice(index, 1);
        setInvoiceItems(updated);

        setFefoResults(prev => prev.filter(r => {
            const removedMedicineID = invoiceItems[index].medicineID;
            return r.medicineID !== +removedMedicineID;
        }));
    };

    // handle change of medicine selection
    // B1: reset item có index thay đổi
    //      gọi fefo API để lấy batch mới
    // B2: lọc list item và 
    //      cập nhật fefoResults
    const handleMedicineChange = async (index, medicineID) => {
        const updated = [...invoiceItems];
        updated[index].medicineID = medicineID;
        updated[index].batchID = null;
        updated[index].unitName = '';
        updated[index].fefoDisplay = '';
        updated[index].availableBatches = [];

        const medicine = listMedicine.find(m => m.medicineID === +medicineID);
        if (medicine) {
            updated[index].unitPrice = medicine.defaultRetailPrice;
            updated[index].unitID = medicine.baseUnitID;
        }

        if (medicineID) {
            const quantity = updated[index].quantity || 1;
            const res = await GetFefoBatches(medicineID, quantity);
            if (res && res.ec === 0) {
                const fefoData = res.dt;
                updated[index].batchID = fefoData.allocatedBatches.length > 0 ? fefoData.allocatedBatches[0].batchID : null;
                updated[index].fefoDisplay = fefoData.allocatedBatches.map(b => `Batch ${b.batchNumber} (${b.quantityAllocated})`).join(', ');

                // filter item có medicineID khác với medicineID vừa chọn để giữ lại
                //  và thêm fefoData mới vào fefoResults
                setFefoResults(prev => {
                    const filtered = prev.filter(r => r.medicineID !== +medicineID);
                    return [...filtered, fefoData];
                });
            }
        }

        setInvoiceItems(updated);
    };

    const handleQuantityChange = async (index, quantity) => {
        const updated = [...invoiceItems];
        updated[index].quantity = +quantity;

        if (updated[index].medicineID) {
            const res = await GetFefoBatches(updated[index].medicineID, +quantity);
            if (res && res.ec === 0) {
                const fefoData = res.dt;
                updated[index].batchID = fefoData.allocatedBatches.length > 0 ? fefoData.allocatedBatches[0].batchID : null;
                updated[index].fefoDisplay = fefoData.allocatedBatches.map(b => `Batch ${b.batchNumber} (${b.quantityAllocated})`).join(', ');

                setFefoResults(prev => {
                    const filtered = prev.filter(r => r.medicineID !== +updated[index].medicineID);
                    return [...filtered, fefoData];
                });
            }
        }

        setInvoiceItems(updated);
    };

    const handleUnitChange = (index, unitID) => {
        const updated = [...invoiceItems];
        updated[index].unitID = +unitID;
        setInvoiceItems(updated);
    };

    const formatPrice = (price) => {
        if (price == null) return '0';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    const getTotalAmount = () => {
        return invoiceItems.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
    };

    // số tiền sẽ trả bằng ví khi khách đồng ý (auto-dùng toàn bộ ví, tối đa = tổng tiền)
    const getWalletAmount = () => {
        if (!useWallet) return 0;
        const total = getTotalAmount();
        return Math.min(customerWalletBalance, total);
    };

    // số nợ còn lại sau khi trừ ví và tiền mặt
    const getRemainingDebt = () => {
        const cash = +paidAmount || 0;
        return Math.max(0, getTotalAmount() - getWalletAmount() - cash);
    };

    // handle submit invoice
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
        }

        const submitItems = invoiceItems.map(item => ({
            MedicineID: +item.medicineID,
            BatchID: null,
            UnitID: +item.unitID,
            Quantity: +item.quantity,
            UnitPrice: +item.unitPrice
        }));

        const walletAmount = getWalletAmount();
        const cashPaid = +paidAmount || 0;
        const total = getTotalAmount();

        if (cashPaid + walletAmount > total) {
            toast.error('Số tiền trả (ví + tiền mặt) không được vượt quá tổng tiền hóa đơn');
            return;
        }

        let res = await CreateInvoice(
            +customerID,
            note,
            canChangeUser && createdByUserID ? +createdByUserID : null,
            submitItems,
            'FEFO',
            cashPaid,
            walletAmount
        );

        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Tạo hóa đơn thất bại');
            return;
        }

        toast.success(res.EM || 'Tạo hóa đơn thành công');
        handleClose();
        await props.fetchListInvoice(1);
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-invoice'>
                <Modal.Header closeButton>
                    <Modal.Title>Thêm hóa đơn mới</Modal.Title>
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
                                    <select className="form-control" value={customerID} onChange={(event) => handleCustomerChange(event.target.value)}>
                                        <option value="">-- Chọn khách hàng --</option>
                                        {listCustomer && listCustomer.length > 0 &&
                                            listCustomer.map((item, index) => (
                                                <option key={`option-cus-${index}`} value={item.customerID}>{item.customerName}</option>
                                            ))
                                        }
                                    </select>
                                    {customerWalletBalance > 0 && (
                                        <div className="customer-wallet-alert mt-2">
                                            <span className="badge bg-warning">⚠️ Khách hàng đang có {formatPrice(customerWalletBalance)}đ tiền thừa trong ví</span>
                                        </div>
                                    )}
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Người lập</label>
                                    <select
                                        className="form-control"
                                        value={createdByUserID || account.userId}
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
                                <div className="col-12 payment-section">
                                    <h6 className="fw-bold mb-2">Thanh toán</h6>
                                    <div className="row g-3">
                                        <div className="col-md-4">
                                            <label className="d-flex align-items-center gap-2">
                                                <input
                                                    type="checkbox"
                                                    className="form-check-input"
                                                    checked={useWallet}
                                                    disabled={customerWalletBalance <= 0}
                                                    onChange={(e) => setUseWallet(e.target.checked)}
                                                />
                                                <span>Sử dụng ví thanh toán</span>
                                            </label>
                                            <div className="text-muted small">
                                                {customerWalletBalance > 0 ? `Số dư ví: ${formatPrice(customerWalletBalance)}đ` : 'Khách không có tiền thừa trong ví'}
                                            </div>
                                            {useWallet && (
                                                <div className="wallet-use-info mt-1">
                                                    Sẽ trả bằng ví: <span className="fw-bold text-success">{formatPrice(getWalletAmount())}đ</span>
                                                </div>
                                            )}
                                        </div>
                                        <div className="col-md-4">
                                            <label>Trả trước bằng tiền mặt (₫)</label>
                                            <input
                                                type="number"
                                                className="form-control"
                                                min="0"
                                                value={paidAmount}
                                                onChange={(event) => setPaidAmount(event.target.value)}
                                            />
                                        </div>
                                        <div className="col-md-4 payment-summary">
                                            <div className="d-flex justify-content-between">
                                                <span>Tổng tiền hàng:</span>
                                                <span className="fw-bold">{formatPrice(getTotalAmount())}đ</span>
                                            </div>
                                            <div className="d-flex justify-content-between text-success">
                                                <span>Trả bằng ví:</span>
                                                <span className="fw-bold">-{formatPrice(getWalletAmount())}đ</span>
                                            </div>
                                            <div className="d-flex justify-content-between">
                                                <span>Trả tiền mặt:</span>
                                                <span className="fw-bold">-{formatPrice(+paidAmount || 0)}đ</span>
                                            </div>
                                            <div className="d-flex justify-content-between fw-bold">
                                                <span>Còn nợ:</span>
                                                <span className="text-warning">{formatPrice(getRemainingDebt())}đ</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </Tab>

                        <Tab eventKey="invoiceItems" title="Chi tiết hóa đơn">
                            <div className='invoice-items-tab'>
                                <div className='d-flex justify-content-between align-items-center mb-2'>
                                    <div>
                                        <span className='fw-bold'>Chế độ: </span>
                                        <span className='badge bg-success'>FEFO - First Expired First Out</span>
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
                                            <th style={{ width: '12%' }}>Đơn giá</th>
                                            <th style={{ width: '12%' }}>Đơn vị</th>
                                            <th style={{ width: '8%' }}>Số lượng</th>
                                            <th style={{ width: '20%' }}>Lô (Batch)</th>
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
                                                        <input
                                                            type="text"
                                                            className="form-control form-control-sm"
                                                            value={item.fefoDisplay}
                                                            disabled
                                                            placeholder="Tự động chọn theo FEFO"
                                                        />
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

                                {fefoResults.length > 0 && (
                                    <div className="fefo-result-section mt-3">
                                        <h6>Kết quả FEFO - Phân bổ lô tự động</h6>
                                        <table className="table table-sm table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Thuốc</th>
                                                    <th>SL yêu cầu</th>
                                                    <th>Lô được chọn</th>
                                                    <th>SL phân bổ</th>
                                                    <th>Hạn sử dụng</th>
                                                    <th>Trạng thái</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {fefoResults.map((result, rIdx) => (
                                                    result.allocatedBatches.map((batch, bIdx) => (
                                                        <tr key={`fefo-${rIdx}-${bIdx}`}>
                                                            {bIdx === 0 && <td rowSpan={result.allocatedBatches.length}>{result.medicineName}</td>}
                                                            {bIdx === 0 && <td rowSpan={result.allocatedBatches.length}>{result.totalQuantityRequested}</td>}
                                                            <td>Batch {batch.batchNumber}</td>
                                                            <td>{batch.quantityAllocated}</td>
                                                            <td>{new Date(batch.expiryDate).toLocaleDateString('vi-VN')}</td>
                                                            {bIdx === 0 && <td rowSpan={result.allocatedBatches.length}>
                                                                {result.fulfilled ?
                                                                    <span className="badge bg-success">Đủ</span> :
                                                                    <span className="badge bg-warning">Thiếu</span>
                                                                }
                                                            </td>}
                                                        </tr>
                                                    ))
                                                ))}
                                            </tbody>
                                        </table>
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
                    <Button variant="primary" onClick={() => handleSubmitInvoice()}>
                        Lưu
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalCreateInvoiceFEFO;
