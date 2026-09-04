import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import {
    CreateStockTake,
    getAllBatchInWarehouse,
    getWarehouseByBranch
} from '../../../services/apiService';

const ModalCreateStockTake = (props) => {
    const { show, setShow } = props;

    const currentBranchId = useSelector(state => state.user.account.currentBranchId);

    const [warehouseID, setWarehouseID] = useState('');
    const [note, setNote] = useState('');

    const [listWarehouse, setListWarehouse] = useState([]);
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show) {
            fetchWarehouses();
        }
    }, [show]);

    const fetchWarehouses = async () => {
        let listWarehouse = props.listWarehouse || [];
        setListWarehouse(listWarehouse);

        fetchAllBatches();
    };

    const fetchAllBatches = async () => {
        setLoading(true);
        let all = [];
        let page = 1;
        let totalPage = 1;
        try {
            do {
                let res = await getAllBatchInWarehouse(page, 100);
                if (res && res.ec === 0 && res.dt) {
                    all = all.concat(res.dt.batches || []);
                    totalPage = res.dt.totalPage || 1;
                } else {
                    break;
                }
                page++;
            } while (page <= totalPage);
        } catch (error) {
            all = [];
        }

        setItems(
            all.map(
                batch => ({
                    batchID: batch.batchID,
                    medicineName: batch.medicineName,
                    unitName: batch.unitName,
                    systemQuantity: batch.quantityInStock,
                    actualQuantity: batch.quantityInStock,
                    adjustQuantity: 0,
                    destroyQuantity: 0
                })
            )
        );
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

    const handleSubmitStockTake = async () => {
        if (!warehouseID || warehouseID === '') {
            toast.error('Vui lòng chọn kho kiểm kê');
            return;
        }
        if (items.length === 0) {
            toast.error('Kho không có lô hàng nào để kiểm kê');
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

        let res = await CreateStockTake(+warehouseID, note, submitItems);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Tạo phiếu kiểm kê thất bại');
            return;
        }

        toast.success(res.em || 'Tạo phiếu kiểm kê thành công');
        handleClose();
        await props.fetchStockTake(1);
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-stock-take'>
                <Modal.Header closeButton>
                    <Modal.Title>Thêm phiếu kiểm kê mới</Modal.Title>
                </Modal.Header>
                <Modal.Body>
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

                    <div className='d-flex justify-content-between align-items-center mb-2'>
                        <label className='fw-bold mb-0'>Danh sách lô hàng trong kho</label>
                        {loading && <span className="text-muted small">Đang tải...</span>}
                    </div>

                    <table className="table table-bordered table-sm">
                        <thead className="table-light">
                            <tr>
                                <th style={{ width: '35%' }}>Thuốc</th>
                                <th style={{ width: '12%' }}>Đơn vị</th>
                                <th style={{ width: '13%' }}>SL hệ thống</th>
                                <th style={{ width: '16%' }}>SL thực tế</th>
                                <th style={{ width: '12%' }}>Chênh lệch</th>
                                <th style={{ width: '12%' }}>Điều chỉnh</th>
                                <th style={{ width: '12%' }}>Tiêu hủy</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items.length === 0 && !loading &&
                                <tr>
                                    <td colSpan={8} className="text-center text-muted">
                                        Không có lô hàng nào trong kho
                                    </td>
                                </tr>
                            }

                            {items.map((item, index) => {
                                const actual = parseFloat(item.actualQuantity) || 0;
                                const diff = actual - item.systemQuantity;
                                const absDiff = Math.abs(diff);
                                const ok = (parseFloat(item.adjustQuantity) || 0) + (parseFloat(item.destroyQuantity) || 0) === absDiff;
                                return (
                                    <tr key={`item-row-${index}`}>
                                        <td>{item.medicineName}</td>
                                        <td>{item.unitName}</td>
                                        <td>{item.systemQuantity}</td>
                                        <td>
                                            <input
                                                type="number"
                                                className="form-control form-control-sm"
                                                value={item.actualQuantity}
                                                min="0"
                                                placeholder={`Nhập số lượng thực tế`}
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
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitStockTake()} disabled={loading}>
                        Lưu
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalCreateStockTake;
