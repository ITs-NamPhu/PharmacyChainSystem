import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

import { GetDestroyReceiptById } from '../../../services/apiService';
import { getStatusBadge } from '../../../utils/statusTicket';

const ModalViewDestroyReceipt = (props) => {
    const { show, setShow, dataView } = props;

    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataView && dataView.destroyReceiptID) {
            fetchDetail(dataView.destroyReceiptID);
        }
    }, [show, dataView]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetDestroyReceiptById(id);
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

    const formatMoney = (value) => {
        if (value == null || isNaN(+value)) return "—";
        return (+value).toLocaleString("vi-VN", { style: "currency", currency: "VND" });
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-view-destroy-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>Chi tiết phiếu tiêu hủy #{dataView.destroyReceiptID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && detail && (
                    <>
                        <div className="row g-3 mb-3">
                            <div className="col-md-6">
                                <label className="fw-bold">Phiếu kiểm kê</label>
                                <div>#{detail.stockTakeID}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Kho</label>
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
                                    <th>SL tiêu hủy</th>
                                    <th>Đơn giá</th>
                                    <th>Thành tiền</th>
                                    <th>Lý do</th>
                                </tr>
                            </thead>
                            <tbody>
                                {detail.items && detail.items.length > 0 &&
                                    detail.items.map((item, index) => (
                                        <tr key={`view-item-${index}`}>
                                            <td>{item.medicineName}</td>
                                            <td>{item.unitName}</td>
                                            <td>{item.quantity}</td>
                                            <td>{formatMoney(item.unitCost)}</td>
                                            <td>{formatMoney(item.quantity * item.unitCost)}</td>
                                            <td>{item.reasonCode || '—'}</td>
                                        </tr>
                                    ))
                                }
                                {detail.items && detail.items.length === 0 &&
                                    <tr>
                                        <td colSpan={6} className="text-center text-muted">
                                            Không có chi tiết
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </>
                )}

                {!loading && !detail && (
                    <div className="text-center text-muted">Không tìm thấy phiếu tiêu hủy</div>
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

export default ModalViewDestroyReceipt;
