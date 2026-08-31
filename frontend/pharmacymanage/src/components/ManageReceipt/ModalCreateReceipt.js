import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { CreateReceipt, getAllCustomerNoPag } from '../../services/apiService';

const ModalCreateReceipt = (props) => {
    const { show, setShow, handleViewDetail } = props;

    const [customerID, setCustomerID] = useState('');
    const [totalAmount, setTotalAmount] = useState(0);
    const [listCustomer, setListCustomer] = useState([]);

    useEffect(() => {
        fetchListCustomer();
    }, [])

    const fetchListCustomer = async () => {
        let res = await getAllCustomerNoPag();
        if (res && res.ec === 0) {
            setListCustomer(res.dt);
        }
    }

    const handleClose = () => {
        setShow(false);
        setCustomerID('');
        setTotalAmount(0);
    };

    const handleSubmitReceipt = async () => {
        if (!customerID || customerID === '') {
            toast.error('Vui lòng chọn khách hàng');
            return;
        }
        if (!totalAmount || +totalAmount <= 0) {
            toast.error('Số tiền khách đưa phải lớn hơn 0');
            return;
        }

        let res = await CreateReceipt(+customerID, +totalAmount);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Tạo phiếu thu thất bại');
            return;
        }

        toast.success(res.EM || 'Tạo phiếu thu thành công');
        handleClose();
        await props.fetchListReceipt(1);
        if (handleViewDetail) {
            handleViewDetail(res.dt);
        }
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="lg" className='modal-create-receipt'>
                <Modal.Header closeButton>
                    <Modal.Title>Thêm phiếu thu</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label>Khách hàng <span className="text-danger">*</span></label>
                            <select className="form-control" value={customerID} onChange={(event) => setCustomerID(event.target.value)}>
                                <option value="">-- Chọn khách hàng --</option>
                                {listCustomer && listCustomer.length > 0 &&
                                    listCustomer.map((item, index) => (
                                        <option key={`option-cus-${index}`} value={item.customerID}>{item.customerName}</option>
                                    ))
                                }
                            </select>
                        </div>
                        <div className="form-group col-md-6">
                            <label>Số tiền khách đưa <span className="text-danger">*</span></label>
                            <input
                                type="number"
                                className="form-control"
                                min="0"
                                placeholder="Nhập số tiền khách thực đưa"
                                value={totalAmount}
                                onChange={(event) => setTotalAmount(event.target.value)}
                            />
                        </div>
                        <div className="col-12 text-muted">
                            Hệ thống sẽ tự động gạch nợ theo các hóa đơn còn nợ từ cũ đến mới (FIFO).
                            Phần dư (nếu có) sẽ được nạp vào ví khách hàng.
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitReceipt()}>
                        Lưu
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalCreateReceipt;