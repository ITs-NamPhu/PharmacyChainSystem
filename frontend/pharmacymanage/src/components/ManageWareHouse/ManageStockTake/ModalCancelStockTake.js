import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

import { CancelStockTake } from '../../../services/apiService';

const ModalCancelStockTake = (props) => {
    const { show, setShow, dataCancel, fetchStockTake } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleCancel = async () => {
        let res = await CancelStockTake(dataCancel.stockTakeID);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Hủy phiếu kiểm kê thất bại');
            return;
        }
        toast.success(res.em || 'Hủy phiếu kiểm kê thành công');
        handleClose();
        await fetchStockTake(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="lg" className='modal-cancel-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Hủy phiếu kiểm kê
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn hủy phiếu kiểm kê #{dataCancel.stockTakeID} ?
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="danger" onClick={() => handleCancel()}>
                    Hủy phiếu
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default ModalCancelStockTake;
