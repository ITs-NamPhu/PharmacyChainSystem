import { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { MdWallet } from 'react-icons/md';

import { GetCustomerWalletById } from '../../services/apiService';

const ModalCustomerWallet = (props) => {
    const { show, setShow, customer } = props;

    const [wallet, setWallet] = useState(null);

    useEffect(() => {
        if (show && customer && customer.customerID) {
            fetchWallet(customer.customerID);
        }
    }, [show, customer]);

    const fetchWallet = async (customerID) => {
        let res = await GetCustomerWalletById(customerID);
        if (res && res.ec === 0) {
            setWallet(res.dt);
        }
    };

    const handleClose = () => {
        setShow(false);
        setWallet(null);
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

    const refTypeLabel = (refType, refId) => {
        // 0 = RECEIPT (nạp từ phiếu thu), 1 = INVOICE (trừ vào hóa đơn)
        if (refType === 0) return <>Nạp từ <span className="fw-bold">Phiếu thu #{refId}</span></>;
        return <>Trừ vào <span className="fw-bold">Hóa đơn #{refId}</span></>;
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="lg" className='modal-customer-wallet'>
                <Modal.Header closeButton>
                    <Modal.Title>Lịch sử ví - {wallet?.customerName || customer?.customerName || ''}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {wallet && (
                        <>
                            <div className="wallet-balance-card">
                                <div className="wallet-balance-label"><MdWallet /> Số dư ví</div>
                                <div className="wallet-balance-value">{formatPrice(wallet.walletBalance)}đ</div>
                            </div>

                            <h6 className="fw-bold mb-2 mt-3">Lịch sử biến động</h6>
                            <table className="table table-bordered table-sm">
                                <thead className="table-light">
                                    <tr>
                                        <th>Loại</th>
                                        <th>Số tiền</th>
                                        <th>Nguồn gốc</th>
                                        <th>Ngày giờ</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {wallet.history && wallet.history.length > 0 &&
                                        wallet.history.map((item, index) => (
                                            <tr key={`wallet-history-${index}`}>
                                                <td>
                                                    {item.transactionType === 0
                                                        ? <span className="badge bg-success">Nạp vào ví</span>
                                                        : <span className="badge bg-danger">Rút ví trả nợ</span>}
                                                </td>
                                                <td className={item.transactionType === 0 ? 'text-success fw-bold' : 'text-danger fw-bold'}>
                                                    {item.transactionType === 0 ? '+' : '-'}{formatPrice(item.amount)}đ
                                                </td>
                                                <td>{refTypeLabel(item.refType, item.refId)}</td>
                                                <td>{formatDate(item.createDate)}</td>
                                            </tr>
                                        ))
                                    }
                                    {wallet.history && wallet.history.length === 0 &&
                                        <tr>
                                            <td colSpan={4} className="text-center text-muted">Chưa có biến động nào</td>
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
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default ModalCustomerWallet;