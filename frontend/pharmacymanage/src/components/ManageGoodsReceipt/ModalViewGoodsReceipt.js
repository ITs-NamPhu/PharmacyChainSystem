import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

import { GetGoodsReceiptById } from '../../services/apiService';
import { getStatusBadge } from '../../utils/statusTicket';

const ModalViewGoodsReceipt = (props) => {
    const { show, setShow, dataView } = props;

    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataView && dataView.goodsReceiptID) {
            fetchDetail(dataView.goodsReceiptID);
        }
    }, [show, dataView]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetGoodsReceiptById(id);
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

    const formatCurrency = (amount) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-view-goods-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>Chi tiết phiếu nhập kho #{dataView.goodsReceiptID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}

                {!loading && detail && (
                    <>
                        <div className="row g-3 mb-3">
                            <div className="col-md-6">
                                <label className="fw-bold">Số phiếu</label>
                                <div>{detail.receiptNumber}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Nhà cung cấp</label>
                                <div>{detail.supplierName}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Người tạo</label>
                                <div>{detail.userName}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Ngày nhập</label>
                                <div>{formatDate(detail.receiptDate)}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Tổng tiền</label>
                                <div>{formatCurrency(detail.totalAmount)}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Đã trả</label>
                                <div>{formatCurrency(detail.paidAmount)}</div>
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
                                    <th>Số lượng</th>
                                    <th>Hệ số</th>
                                    <th>Đơn giá</th>
                                    <th>Thành tiền</th>
                                    <th>SL nhập kho</th>
                                    <th>SL tồn kho</th>
                                    <th>Ngày SX</th>
                                    <th>Hạn SD</th>
                                </tr>
                            </thead>
                            <tbody>
                                {detail.items && detail.items.length > 0 &&
                                    detail.items.map((item, index) => {
                                        const subTotal = (item.quantity || 0) * (item.unitCost || 0);
                                        return (
                                            <tr key={`view-item-${index}`}>
                                                <td>{item.medicineName}</td>
                                                <td>{item.unitName}</td>
                                                <td>{item.quantity}</td>
                                                <td>{item.conversionFactor}</td>
                                                <td>{formatCurrency(item.unitCost)}</td>
                                                <td>{formatCurrency(subTotal)}</td>
                                                <td>{item.quantityReceived}</td>
                                                <td>{item.quantityInStock}</td>
                                                <td>{item.manufactureDate ? formatDate(item.manufactureDate) : '—'}</td>
                                                <td>{item.expiryDate ? formatDate(item.expiryDate) : '—'}</td>
                                            </tr>
                                        );
                                    })
                                }
                                {detail.items && detail.items.length === 0 &&
                                    <tr>
                                        <td colSpan={10} className="text-center text-muted">
                                            Không có chi tiết
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </>
                )}

                {!loading && !detail && (
                    <div className="text-center text-muted">Không tìm thấy phiếu nhập kho</div>
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

export default ModalViewGoodsReceipt;
