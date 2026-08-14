import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

import { ApproveStockTake } from '../../../services/apiService';

const ModalApproveStockTake = (props) => {
    const { show, setShow, dataApprove, fetchStockTake } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleApprove = async () => {
        let res = await ApproveStockTake(dataApprove.stockTakeID);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Duyệt phiếu kiểm kê thất bại');
            return;
        }
        toast.success(res.em || 'Duyệt phiếu kiểm kê thành công');
        handleClose();
        await fetchStockTake(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="lg" className='modal-approve-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Duyệt phiếu kiểm kê
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn duyệt phiếu kiểm kê #{dataApprove.stockTakeID} ?
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

export default ModalApproveStockTake;
