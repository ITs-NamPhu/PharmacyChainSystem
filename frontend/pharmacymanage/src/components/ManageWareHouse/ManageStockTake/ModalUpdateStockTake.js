import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetStockTakeById, UpdateStockTake } from '../../../services/apiService';

const ModalUpdateStockTake = (props) => {
    const { show, setShow, dataUpdate, listWarehouse, fetchStockTake } = props;

    const [warehouseID, setWarehouseID] = useState('');
    const [note, setNote] = useState('');
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataUpdate && dataUpdate.stockTakeID) {
            fetchDetail(dataUpdate.stockTakeID);
        }
    }, [show, dataUpdate]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetStockTakeById(id);
        if (res && res.ec === 0 && res.dt) {
            const detail = res.dt;
            setWarehouseID(detail.warehouseID || '');
            setNote(detail.note || '');
            setItems((detail.items || []).map(item => ({
                batchID: item.batchID,
                medicineName: item.medicineName,
                unitName: item.unitName,
                systemQuantity: item.systemQuantity,
                actualQuantity: item.actualQuantity,
                adjustQuantity: item.adjustQuantity,
                destroyQuantity: item.destroyQuantity
            })));
        } else {
            setItems([]);
        }
        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setWarehouseID('');
        setNote('');
        setItems([]);
    };

    const handleChangeItem = (index, value) => {
        const updated = [...items];
        updated[index].actualQuantity = value;
        const actual = parseFloat(value) || 0;
        const diff = actual - updated[index].systemQuantity;
        const absDiff = Math.abs(diff);
        updated[index].adjustQuantity = absDiff;
        updated[index].destroyQuantity = 0;
        setItems(updated);
    };

    const handleReviewQuantity = (index, field, value) => {
        const updated = [...items];
        updated[index][field] = value;
        setItems(updated);
    };

    const handleSubmitUpdate = async () => {
        if (!warehouseID || warehouseID === '') {
            toast.error('Vui lòng chọn kho kiểm kê');
            return;
        }
        if (items.length === 0) {
            toast.error('Phiếu kiểm kê không có chi tiết nào');
            return;
        }

        for (let i = 0; i < items.length; i++) {
            const qty = items[i].actualQuantity;
            if (qty === '' || qty == null || isNaN(+qty) || +qty < 0) {
                toast.error(`Số lượng thực tế phải >= 0 tại dòng ${i + 1}`);
                return;
            }
            const actual = +items[i].actualQuantity;
            const diff = actual - items[i].systemQuantity;
            const absDiff = Math.abs(diff);
            const adjust = +items[i].adjustQuantity || 0;
            const destroy = +items[i].destroyQuantity || 0;
            if (adjust < 0 || destroy < 0) {
                toast.error(`Số lượng điều chỉnh/tiêu hủy phải >= 0 tại dòng ${i + 1}`);
                return;
            }
            if (adjust + destroy !== absDiff) {
                toast.error(`Dòng ${i + 1}: Điều chỉnh + Tiêu hủy phải bằng |Chênh lệch| (${absDiff})`);
                return;
            }
        }

        const submitItems = items.map(item => ({
            BatchID: +item.batchID,
            ActualQuantity: +item.actualQuantity,
            AdjustQuantity: +item.adjustQuantity || 0,
            DestroyQuantity: +item.destroyQuantity || 0
        }));

        let res = await UpdateStockTake(dataUpdate.stockTakeID, {
            WarehouseID: +warehouseID,
            Note: note,
            Items: submitItems
        });
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Cập nhật phiếu kiểm kê thất bại');
            return;
        }

        toast.success(res.em || 'Cập nhật phiếu kiểm kê thành công');
        handleClose();
        await fetchStockTake(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-update-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>Sửa phiếu kiểm kê #{dataUpdate.stockTakeID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && (
                    <>
                        <form className='row g-3 mb-3'>
                            <div className="form-group col-md-6">
                                <label>
                                    Kho kiểm kê <span className="text-danger">*</span>
                                </label>
                                <select
                                    className="form-control"
                                    value={warehouseID}
                                    onChange={(event) => setWarehouseID(event.target.value)}
                                >
                                    <option value="">-- Chọn kho --</option>
                                    {listWarehouse && listWarehouse.length > 0 &&
                                        listWarehouse.map((item, index) => (
                                            <option key={`option-warehouse-${index}`} value={item.warehouseID}>
                                                {item.warehouseName}
                                            </option>
                                        ))
                                    }
                                </select>
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

                        <table className="table table-bordered table-sm">
                            <thead className="table-light">
                                <tr>
                                    <th>Thuốc</th>
                                    <th>Đơn vị</th>
                                    <th>SL hệ thống</th>
                                    <th>SL thực tế</th>
                                    <th>Chênh lệch</th>
                                    <th>Điều chỉnh</th>
                                    <th>Tiêu hủy</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((item, index) => {
                                    const actual = parseFloat(item.actualQuantity) || 0;
                                    const diff = actual - item.systemQuantity;
                                    const absDiff = Math.abs(diff);
                                    const ok = (parseFloat(item.adjustQuantity) || 0) + (parseFloat(item.destroyQuantity) || 0) === absDiff;
                                    return (
                                        <tr key={`update-item-${index}`}>
                                            <td>{item.medicineName}</td>
                                            <td>{item.unitName}</td>
                                            <td>{item.systemQuantity}</td>
                                            <td>
                                                <input
                                                    type="number"
                                                    className="form-control form-control-sm"
                                                    value={item.actualQuantity}
                                                    min="0"
                                                    onChange={(event) =>
                                                        handleChangeItem(index, event.target.value)
                                                    }
                                                />
                                            </td>
                                            <td>
                                                <span className={`fw-bold ${diff === 0 ? 'text-success' : (diff > 0 ? 'text-primary' : 'text-danger')}`}>
                                                    {diff > 0 ? `+${diff}` : diff}
                                                </span>
                                            </td>
                                            <td>
                                                <input
                                                    type="number"
                                                    className="form-control form-control-sm"
                                                    value={item.adjustQuantity}
                                                    min="0"
                                                    disabled={diff === 0}
                                                    onChange={(event) =>
                                                        handleReviewQuantity(index, 'adjustQuantity', event.target.value)
                                                    }
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    type="number"
                                                    className="form-control form-control-sm"
                                                    value={item.destroyQuantity}
                                                    min="0"
                                                    disabled={diff === 0}
                                                    onChange={(event) =>
                                                        handleReviewQuantity(index, 'destroyQuantity', event.target.value)
                                                    }
                                                />
                                            </td>
                                            <td className="text-center">
                                                {absDiff !== 0 && !ok && <span className="text-danger small">≠ |Diff|</span>}
                                                {absDiff !== 0 && ok && <span className="text-success small">OK</span>}
                                            </td>
                                        </tr>
                                    );
                                })}
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

export default ModalUpdateStockTake;
