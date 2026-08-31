import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { MdWallet } from 'react-icons/md';
import { toast } from 'react-toastify';

import { GetReceiptById } from '../../services/apiService';

const ModalReceiptDetail = (props) => {
    const { show, setShow, data } = props;

    const [detail, setDetail] = useState(null);

    useEffect(() => {
        if (show && data && data.receiptID) {
            fetchDetail(data.receiptID);
        }
    }, [show, data]);

    const fetchDetail = async (id) => {
        let res = await GetReceiptById(id);
        if (res && res.ec === 0) {
            setDetail(res.dt);
        } else {
            toast.error(res?.EM || 'Không thể tải chi tiết phiếu thu');
        }
    };

    const formatPrice = (price) => {
        if (price == null) return '0';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        return d.toLocaleString('vi-VN');
    };

    const handleClose = () => {
        setShow(false);
        setDetail(null);
    };

    const paymentMethod = detail?.paymentMethod === 1 ? 'Ví' : 'Tiền mặt';
    const walletCredit = detail ? +(detail.walletCredit || 0) : 0;
    const hasExcess = walletCredit > 0;

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-receipt-detail'>
                <Modal.Header closeButton>
                    <Modal.Title>
                        Chi tiết phiếu thu #{detail?.receiptID || ''}
                        {detail?.customerName && <span className="text-muted fs-6 ms-2">- {detail.customerName}</span>}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {detail && (
                        <>
                            <div className="receipt-summary-card">
                                <div className="summary-row">
                                    <span className="summary-label">Tổng tiền thu (Receipt Amount)</span>
                                    <span className="summary-value summary-total">{formatPrice(detail.totalAmount)}đ</span>
                                </div>
                                <div className="summary-row">
                                    <span className="summary-label">Đã phân bổ (Applied to Invoices)</span>
                                    <span className="summary-value">{formatPrice(detail.amountApplied)}đ</span>
                                </div>
                                {hasExcess ? (
                                    <div className="summary-row receipt-wallet">
                                        <span className="summary-label"><MdWallet /> Chuyển vào ví / Tiền thừa (To Wallet)</span>
                                        <span className="summary-value">+{formatPrice(walletCredit)}đ</span>
                                    </div>
                                ) : (
                                    <div className="summary-row receipt-wallet-zero">
                                        <span className="summary-label"><MdWallet /> Chuyển vào ví / Tiền thừa (To Wallet)</span>
                                        <span className="summary-value">0đ</span>
                                    </div>
                                )}
                                <div className="summary-row summary-divider">
                                    <span className="summary-label">Phương thức thanh toán</span>
                                    <span className="summary-value">{paymentMethod}</span>
                                </div>
                                <div className={`summary-row ${detail.remainingDebt > 0 ? 'receipt-debt' : ''}`}>
                                    <span className="summary-label">Còn nợ sau phiếu thu (Remaining Debt)</span>
                                    <span className="summary-value">{formatPrice(detail.remainingDebt)}đ</span>
                                </div>
                                <div className="summary-row">
                                    <span className="summary-label">Ngày tạo</span>
                                    <span className="summary-value">{formatDate(detail.createdDate)}</span>
                                </div>
                                <div className="summary-row">
                                    <span className="summary-label">Người lập</span>
                                    <span className="summary-value">{detail.userName}</span>
                                </div>
                            </div>

                            <h6 className="fw-bold mb-2">Chi tiết gạch nợ (Receipt Details)</h6>
                            <div className="receipt-details-table">
                                <table className="table table-bordered table-sm">
                                    <thead className="table-light">
                                        <tr>
                                            <th>#</th>
                                            <th>Mã hóa đơn</th>
                                            <th>Ngày hóa đơn</th>
                                            <th>Tổng tiền hóa đơn</th>
                                            <th>Số tiền phân bổ</th>
                                            <th>Trạng thái thanh toán</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {detail.details && detail.details.length > 0 &&
                                            detail.details.map((item, index) => (
                                                <tr key={`receipt-detail-${index}`}>
                                                    <td>{index + 1}</td>
                                                    <td>{item.invoiceID}</td>
                                                    <td>{formatDate(item.invoiceCreatedAt)}</td>
                                                    <td>{formatPrice(item.invoiceTotalAmount)}đ</td>
                                                    <td className="fw-bold">{formatPrice(item.amountApplied)}đ</td>
                                                    <td>
                                                        {item.invoicePaymentStatus === 1
                                                            ? <span className="badge bg-success">Đã trả hết</span>
                                                            : <span className="badge bg-warning">Còn nợ</span>}
                                                    </td>
                                                </tr>
                                            ))
                                        }
                                        {detail.details && detail.details.length === 0 &&
                                            <tr>
                                                <td colSpan={6} className="text-center text-muted">Không có dòng gạch nợ nào</td>
                                            </tr>
                                        }
                                    </tbody>
                                    {detail.details && detail.details.length > 0 &&
                                        <tfoot>
                                            <tr className="table-info">
                                                <td colSpan={4} className="text-end fw-bold">Tổng đã phân bổ:</td>
                                                <td className="fw-bold">{formatPrice(detail.amountApplied)}đ</td>
                                                <td></td>
                                            </tr>
                                        </tfoot>
                                    }
                                </table>
                            </div>
                        </>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default ModalReceiptDetail;