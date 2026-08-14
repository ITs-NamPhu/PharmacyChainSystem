import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

import { DeleteGoodsReceipt } from '../../services/apiService';

const ModalDeleteGoodsReceipt = (props) => {
    const { show, setShow, dataDelete, fetchListGoodsReceipt, setCurrentPage } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleDelete = async () => {
        let res = await DeleteGoodsReceipt(dataDelete.goodsReceiptID);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Xóa phiếu nhập kho thất bại');
            return;
        }
        toast.success(res.EM || 'Xóa phiếu nhập kho thành công');
        handleClose();
        await fetchListGoodsReceipt(1);
        setCurrentPage(1);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-delete-goods-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Xóa phiếu nhập kho
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn xóa phiếu nhập kho #{dataDelete.receiptNumber} ?
            </Modal.Body>

            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Close
                </Button>
                <Button variant="danger" onClick={() => handleDelete()}>
                    Delete
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default ModalDeleteGoodsReceipt;
