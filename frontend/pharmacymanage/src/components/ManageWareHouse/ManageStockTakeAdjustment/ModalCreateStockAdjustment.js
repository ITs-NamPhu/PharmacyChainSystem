import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import {
    CreateStockAdjustment,
    getAllStockTakePag,
    GetStockTakeById
} from '../../../services/apiService';

const ModalCreateStockAdjustment = (props) => {
    const { show, setShow, warehouseID } = props;

    const [stockTakeID, setStockTakeID] = useState('');
    const [note, setNote] = useState('');

    const [listStockTake, setListStockTake] = useState([]);
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show) {
            fetchEligibleStockTakes();
        }
    }, [show]);

    const fetchEligibleStockTakes = async () => {
        setLoading(true);
        let all = [];
        let page = 1;
        let totalPage = 1;
        try {
            do {
                let res = await getAllStockTakePag(page, 100, warehouseID);
                if (res && res.ec === 0 && res.dt) {
                    all = all.concat(res.dt.stockTakes || []);
                    totalPage = res.dt.totalPage || 1;
                } else {
                    break;
                }
                page++;
            } while (page <= totalPage);
        } catch (error) {
            all = [];
        }

        const eligible = all.filter(st =>
            st.status === 'Completed' &&
            st.isBalance === 'Difference' &&
            !st.isAdjusted
        );

        setListStockTake(eligible);
        setLoading(false);
    };

    const handleChangeStockTake = async (event) => {
        const value = event.target.value;
        setStockTakeID(value);

        if (!value) {
            setItems([]);
            return;
        }

        setLoading(true);
        let res = await GetStockTakeById(+value);
        if (res && res.ec === 0 && res.dt) {
            const detail = res.dt;
            setItems(
                (detail.items || [])
                    .filter(item => item.differenceQuantity !== 0)
                    .map(item => ({
                        stockTakeItemID: item.stockTakeItemID,
                        batchID: item.batchID,
                        medicineName: item.medicineName,
                        unitName: item.unitName,
                        systemQuantity: item.systemQuantity,
                        actualQuantity: item.actualQuantity,
                        adjustQuantity: item.differenceQuantity,
                        reasonCode: ''
                    }))
            );
        } else {
            setItems([]);
        }
        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setStockTakeID('');
        setNote('');
        setItems([]);
    };

    const handleChangeItem = (index, field, value) => {
        const updated = [...items];
        updated[index][field] = value;
        setItems(updated);
    };

    const handleSubmitStockAdjustment = async () => {
        if (!stockTakeID || stockTakeID === '') {
            toast.error('Vui lòng chọn phiếu kiểm kê để điều chỉnh');
            return;
        }
        if (items.length === 0) {
            toast.error('Phiếu kiểm kê không có chênh lệch nào cần điều chỉnh');
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
            StockTakeItemID: +item.stockTakeItemID,
            AdjustQuantity: +item.adjustQuantity,
            ReasonCode: item.reasonCode || null
        }));

        let res = await CreateStockAdjustment(+stockTakeID, note, submitItems);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Tạo phiếu điều chỉnh tồn kho thất bại');
            return;
        }

        toast.success(res.em || 'Tạo phiếu điều chỉnh tồn kho thành công');
        handleClose();
        await props.fetchStockAdjustment(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-create-stock-adjustment'>
            <Modal.Header closeButton>
                <Modal.Title>Tạo phiếu điều chỉnh tồn kho</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form className='row g-3 mb-3'>
                    <div className="form-group col-md-6">
                        <label>
                            Phiếu kiểm kê <span className="text-danger">*</span>
                        </label>
                        <select
                            className="form-control"
                            value={stockTakeID}
                            onChange={handleChangeStockTake}
                        >
                            <option value="">-- Chọn phiếu kiểm kê --</option>
                            {listStockTake && listStockTake.length > 0 &&
                                listStockTake.map((item, index) => (
                                    <option key={`option-stockTake-${index}`} value={item.stockTakeID}>
                                        #{item.stockTakeID} - {item.warehouseName} ({item.userName})
                                    </option>
                                ))
                            }
                        </select>
                        {listStockTake && listStockTake.length === 0 && !loading &&
                            <small className="text-muted">
                                Không có phiếu kiểm kê nào hoàn thành và lệch tồn kho chưa điều chỉnh
                            </small>
                        }
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
                    <label className='fw-bold mb-0'>Danh sách lô hàng chênh lệch (SL điều chỉnh lấy từ chênh lệch kiểm kê)</label>
                    {loading && <span className="text-muted small">Đang tải...</span>}
                </div>

                <table className="table table-bordered table-sm">
                    <thead className="table-light">
                        <tr>
                            <th style={{ width: '30%' }}>Thuốc</th>
                            <th style={{ width: '10%' }}>Đơn vị</th>
                            <th style={{ width: '13%' }}>SL hệ thống</th>
                            <th style={{ width: '13%' }}>SL thực tế</th>
                            <th style={{ width: '16%' }}>SL điều chỉnh</th>
                            <th style={{ width: '18%' }}>Lý do</th>
                        </tr>
                    </thead>
                    <tbody>
                        {items.length === 0 && !loading &&
                            <tr>
                                <td colSpan={6} className="text-center text-muted">
                                    {stockTakeID ? 'Phiếu kiểm kê không có chênh lệch' : 'Chọn phiếu kiểm kê để hiển thị'}
                                </td>
                            </tr>
                        }

                        {items.map((item, index) => (
                            <tr key={`item-row-${index}`}>
                                <td>{item.medicineName}</td>
                                <td>{item.unitName}</td>
                                <td>{item.systemQuantity}</td>
                                <td>{item.actualQuantity}</td>
                                <td>
                                    <input
                                        type="number"
                                        className="form-control form-control-sm"
                                        value={item.adjustQuantity}
                                        placeholder="SL điều chỉnh"
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
                                        placeholder="Lý do điều chỉnh"
                                        onChange={(event) =>
                                            handleChangeItem(index, 'reasonCode', event.target.value)
                                        }
                                    />
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="primary" onClick={() => handleSubmitStockAdjustment()} disabled={loading}>
                    Lưu
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalCreateStockAdjustment;
