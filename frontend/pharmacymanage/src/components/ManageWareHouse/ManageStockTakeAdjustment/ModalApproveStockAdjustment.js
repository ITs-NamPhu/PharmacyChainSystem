import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

import { ApproveStockAdjustment } from '../../../services/apiService';

const ModalApproveStockAdjustment = (props) => {
    const { show, setShow, dataApprove, fetchStockAdjustment } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleApprove = async () => {
        let res = await ApproveStockAdjustment(dataApprove.stockAdjustmentID);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Duyệt phiếu điều chỉnh tồn kho thất bại');
            return;
        }
        toast.success(res.em || 'Duyệt phiếu điều chỉnh tồn kho thành công');
        handleClose();
        await fetchStockAdjustment(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="lg" className='modal-approve-stock-adjustment'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Duyệt phiếu điều chỉnh tồn kho
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn duyệt phiếu điều chỉnh #{dataApprove.stockAdjustmentID} ?
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

export default ModalApproveStockAdjustment;
