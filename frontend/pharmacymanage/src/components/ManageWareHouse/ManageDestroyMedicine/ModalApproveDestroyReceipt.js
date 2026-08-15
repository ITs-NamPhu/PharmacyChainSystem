import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

import { ApproveDestroyReceipt } from '../../../services/apiService';

const ModalApproveDestroyReceipt = (props) => {
    const { show, setShow, dataApprove, fetchDestroyReceipt } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleApprove = async () => {
        let res = await ApproveDestroyReceipt(dataApprove.destroyReceiptID);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Duyệt phiếu tiêu hủy thất bại');
            return;
        }
        toast.success(res.em || 'Duyệt phiếu tiêu hủy thành công');
        handleClose();
        await fetchDestroyReceipt(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="lg" className='modal-approve-destroy-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Duyệt phiếu tiêu hủy
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn duyệt phiếu tiêu hủy #{dataApprove.destroyReceiptID} ?
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="primary" onClick={() => handleApprove()}>
                    Duyệt
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default ModalApproveDestroyReceipt;
