import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

import { GetStockTakeById } from '../../../services/apiService';
import { getStatusBadge } from '../../../utils/statusTicket';

const ModalViewStockTake = (props) => {
    const { show, setShow, dataView } = props;

    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataView && dataView.stockTakeID) {
            fetchDetail(dataView.stockTakeID);
        }
    }, [show, dataView]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetStockTakeById(id);
        if (res && res.ec === 0 && res.dt) {
            setDetail(res.dt);
        } else {
            setDetail(null);
        }
        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setDetail(null);
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return "";
        const d = new Date(dateStr);
        return d.toLocaleDateString("vi-VN") + " " + d.toLocaleTimeString("vi-VN");
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-view-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>Chi tiết phiếu kiểm kê #{dataView.stockTakeID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && detail && (
                    <>
                        <div className="row g-3 mb-3">
                            <div className="col-md-6">
                                <label className="fw-bold">Kho kiểm kê</label>
                                <div>{detail.warehouseName}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Người tạo</label>
                                <div>{detail.userName}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Ngày tạo</label>
                                <div>{formatDate(detail.createdAt)}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Trạng thái</label>
                                <div>{getStatusBadge(detail.status)}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Người duyệt</label>
                                <div>{detail.approvedBy ? `${detail.approvedBy} (${formatDate(detail.approvedAt)})` : '—'}</div>
                            </div>
                            <div className="col-12">
                                <label className="fw-bold">Ghi chú</label>
                                <div>{detail.note || '—'}</div>
                            </div>
                        </div>

                        <table className="table table-bordered table-sm">
                            <thead className="table-light">
                                <tr>
                                    <th>Thuốc</th>
                                    <th>Đơn vị</th>
                                    <th>SL hệ thống</th>
                                    <th>SL thực tế</th>
                                    <th>Chênh lệch</th>
                                    <th>SL điều chỉnh</th>
                                    <th>SL tiêu hủy</th>
                                </tr>
                            </thead>
                            <tbody>
                                {detail.items && detail.items.length > 0 &&
                                    detail.items.map((item, index) => (
                                        <tr key={`view-item-${index}`}>
                                            <td>{item.medicineName}</td>
                                            <td>{item.unitName}</td>
                                            <td>{item.systemQuantity}</td>
                                            <td>{item.actualQuantity}</td>
                                            <td>
                                                <span className={`fw-bold ${item.differenceQuantity === 0 ? 'text-success' : (item.differenceQuantity > 0 ? 'text-primary' : 'text-danger')}`}>
                                                    {item.differenceQuantity > 0 ? `+${item.differenceQuantity}` : item.differenceQuantity}
                                                </span>
                                            </td>
                                            <td>{item.adjustQuantity}</td>
                                            <td>{item.destroyQuantity}</td>
                                        </tr>
                                    ))
                                }
                                {detail.items && detail.items.length === 0 &&
                                    <tr>
                                        <td colSpan={7} className="text-center text-muted">
                                            Không có chi tiết
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </>
                )}

                {!loading && !detail && (
                    <div className="text-center text-muted">Không tìm thấy phiếu kiểm kê</div>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalViewStockTake;
