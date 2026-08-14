import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetStockTakeById, CompleteStockTake } from '../../../services/apiService';

const ModalCompleteStockTake = (props) => {
    const { show, setShow, dataComplete, fetchStockTake } = props;

    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataComplete && dataComplete.stockTakeID) {
            fetchDetail(dataComplete.stockTakeID);
        }
    }, [show, dataComplete]);

    const fetchDetail = async (id) => {
        setLoading(true);

        let res = await GetStockTakeById(id);
        if (res && res.ec === 0 && res.dt) {
            const detail = res.dt;
            setItems((detail.items || []).map(item => ({
                stockTakeItemID: item.stockTakeItemID,
                medicineName: item.medicineName,
                unitName: item.unitName,
                systemQuantity: item.systemQuantity,
                actualQuantity: item.actualQuantity
            })));
        } else {
            setItems([]);
        }

        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setItems([]);
    };

    const handleChangeItem = (index, value) => {
        const updated = [...items];
        updated[index].actualQuantity = value;
        setItems(updated);
    };

    const handleSubmitComplete = async () => {
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
        }

        const submitItems = items.map(item => ({
            StockTakeItemID: item.stockTakeItemID,
            ActualQuantity: +item.actualQuantity
        }));

        let res = await CompleteStockTake(dataComplete.stockTakeID, submitItems);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Hoàn thành kiểm kê thất bại');
            return;
        }

        toast.success(res.em || 'Hoàn thành kiểm kê thành công');
        handleClose();
        await fetchStockTake(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-complete-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>Hoàn thành phiếu kiểm kê #{dataComplete.stockTakeID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && (
                    <table className="table table-bordered table-sm">
                        <thead className="table-light">
                            <tr>
                                <th>Thuốc</th>
                                <th>Đơn vị</th>
                                <th>SL hệ thống</th>
                                <th>SL thực tế</th>
                                <th>Chênh lệch</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items.length === 0 &&
                                <tr>
                                    <td colSpan={5} className="text-center text-muted">
                                        Không có chi tiết
                                    </td>
                                </tr>
                            }

                            {items.map((item, index) => {
                                const actual = parseFloat(item.actualQuantity) || 0;
                                const diff = actual - item.systemQuantity;
                                return (
                                    <tr key={`complete-item-${index}`}>
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
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="success" onClick={() => handleSubmitComplete()} disabled={loading}>
                    Hoàn thành
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalCompleteStockTake;
