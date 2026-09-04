import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetDestroyReceiptById, UpdateDestroyReceipt } from '../../../services/apiService';

const ModalUpdateDestroyReceipt = (props) => {
    const { show, setShow, dataUpdate, fetchDestroyReceipt } = props;

    const [note, setNote] = useState('');
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataUpdate && dataUpdate.destroyReceiptID) {
            fetchDetail(dataUpdate.destroyReceiptID);
        }
    }, [show, dataUpdate]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetDestroyReceiptById(id);
        if (res && res.ec === 0 && res.dt) {
            const detail = res.dt;
            setNote(detail.note || '');
            setItems((detail.items || []).map(item => ({
                batchID: item.batchID,
                medicineName: item.medicineName,
                unitName: item.unitName,
                quantity: item.quantity,
                unitCost: item.unitCost,
                reasonCode: item.reasonCode || ''
            })));
        } else {
            setItems([]);
        }
        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setNote('');
        setItems([]);
    };

    const handleChangeItem = (index, field, value) => {
        const updated = [...items];
        updated[index][field] = value;
        setItems(updated);
    };

    const handleSubmitUpdate = async () => {
        if (items.length === 0) {
            toast.error('Phiếu tiêu hủy không có chi tiết nào');
            return;
        }

        for (let i = 0; i < items.length; i++) {
            const qty = items[i].quantity;
            if (qty === '' || qty == null || isNaN(+qty) || +qty <= 0) {
                toast.error(`Số lượng tiêu hủy phải > 0 tại dòng ${i + 1}`);
                return;
            }
        }

        const submitItems = items.map(item => ({
            BatchID: +item.batchID,
            Quantity: +item.quantity,
            ReasonCode: item.reasonCode || null
        }));

        let res = await UpdateDestroyReceipt(dataUpdate.destroyReceiptID, {
            WarehouseID: dataUpdate.warehouseID,
            Note: note,
            Items: submitItems
        });
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Cập nhật phiếu tiêu hủy thất bại');
            return;
        }

        toast.success(res.em || 'Cập nhật phiếu tiêu hủy thành công');
        handleClose();
        await fetchDestroyReceipt(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-update-destroy-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>Sửa phiếu tiêu hủy #{dataUpdate.destroyReceiptID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && (
                    <>
                        <form className='row g-3 mb-3'>
                            <div className="form-group col-md-12">
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

                        <table className="table table-bordered table-sm">
                            <thead className="table-light">
                                <tr>
                                    <th>Thuốc</th>
                                    <th>Đơn vị</th>
                                    <th>Đơn giá</th>
                                    <th>SL tiêu hủy</th>
                                    <th>Thành tiền</th>
                                    <th>Lý do</th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((item, index) => (
                                    <tr key={`update-item-${index}`}>
                                        <td>{item.medicineName}</td>
                                        <td>{item.unitName}</td>
                                        <td>{item.unitCost ? (+item.unitCost).toLocaleString("vi-VN", { style: "currency", currency: "VND" }) : '—'}</td>
                                        <td>
                                            <input
                                                type="number"
                                                className="form-control form-control-sm"
                                                value={item.quantity}
                                                min="1"
                                                onChange={(event) =>
                                                    handleChangeItem(index, 'quantity', event.target.value)
                                                }
                                            />
                                        </td>
                                        <td>{item.unitCost ? (+item.quantity * +item.unitCost).toLocaleString("vi-VN", { style: "currency", currency: "VND" }) : '—'}</td>
                                        <td>
                                            <input
                                                type="text"
                                                className="form-control form-control-sm"
                                                value={item.reasonCode}
                                                placeholder="Lý do"
                                                onChange={(event) =>
                                                    handleChangeItem(index, 'reasonCode', event.target.value)
                                                }
                                            />
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="warning" className='text-dark' onClick={() => handleSubmitUpdate()} disabled={loading}>
                    Lưu
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalUpdateDestroyReceipt;
