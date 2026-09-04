import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetStockAdjustmentById, UpdateStockAdjustment } from '../../../services/apiService';

const ModalUpdateStockAdjustment = (props) => {
    const { show, setShow, dataUpdate, fetchStockAdjustment } = props;

    const [note, setNote] = useState('');
    const [warehouseID, setWarehouseID] = useState('');
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataUpdate && dataUpdate.stockAdjustmentID) {
            fetchDetail(dataUpdate.stockAdjustmentID);
        }
    }, [show, dataUpdate]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetStockAdjustmentById(id);
        if (res && res.ec === 0 && res.dt) {
            const detail = res.dt;
            setWarehouseID(detail.warehouseID || '');
            setNote(detail.note || '');
            setItems((detail.items || []).map(item => ({
                stockAdjustmentItemID: item.stockAdjustmentItemID,
                batchID: item.batchID,
                medicineName: item.medicineName,
                unitName: item.unitName,
                adjustQuantity: item.adjustQuantity,
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
        setWarehouseID('');
        setItems([]);
    };

    const handleChangeItem = (index, field, value) => {
        const updated = [...items];
        updated[index][field] = value;
        setItems(updated);
    };

    const handleSubmitUpdate = async () => {
        if (items.length === 0) {
            toast.error('Phiếu điều chỉnh không có chi tiết nào');
            return;
        }

        for (let i = 0; i < items.length; i++) {
            const qty = items[i].adjustQuantity;
            if (qty === '' || qty == null || isNaN(+qty)) {
                toast.error(`Số lượng điều chỉnh không hợp lệ tại dòng ${i + 1}`);
                return;
            }
        }

        const submitItems = items.map(item => ({
            BatchID: +item.batchID,
            AdjustQuantity: +item.adjustQuantity || 0,
            ReasonCode: item.reasonCode || null
        }));

        let res = await UpdateStockAdjustment(dataUpdate.stockAdjustmentID, {
            StockAdjustmentID: +dataUpdate.stockAdjustmentID,
            WarehouseID: +warehouseID,
            Note: note,
            Items: submitItems
        });
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Cập nhật phiếu điều chỉnh thất bại');
            return;
        }

        toast.success(res.em || 'Cập nhật phiếu điều chỉnh thành công');
        handleClose();
        await fetchStockAdjustment(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-update-stock-adjustment'>
            <Modal.Header closeButton>
                <Modal.Title>Sửa phiếu điều chỉnh #{dataUpdate.stockAdjustmentID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && (
                    <>
                        <form className='row g-3 mb-3'>
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

                        <table className="table table-bordered table-sm">
                            <thead className="table-light">
                                <tr>
                                    <th>Thuốc</th>
                                    <th>Đơn vị</th>
                                    <th>SL điều chỉnh</th>
                                    <th>Lý do</th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((item, index) => (
                                    <tr key={`update-item-${index}`}>
                                        <td>{item.medicineName}</td>
                                        <td>{item.unitName}</td>
                                        <td>
                                            <input
                                                type="number"
                                                className="form-control form-control-sm"
                                                value={item.adjustQuantity}
                                                onChange={(event) =>
                                                    handleChangeItem(index, 'adjustQuantity', event.target.value)
                                                }
                                            />
                                        </td>
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
                                {items.length === 0 &&
                                    <tr>
                                        <td colSpan={4} className="text-center text-muted">
                                            Không có chi tiết
                                        </td>
                                    </tr>
                                }
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

export default ModalUpdateStockAdjustment;
