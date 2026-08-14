import { toast } from 'react-toastify';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

import { DeleteInvoice } from '../../services/apiService';

const ModalDeleteInvoice = (props) => {
    const { show, setShow, dataDelete, fetchListInvoice, setCurrentPage } = props;

    const handleClose = () => {
        setShow(false);
    };

    const handleDelete = async () => {
        let res = await DeleteInvoice(dataDelete.invoiceID);
        if (res && res.ec === 0) {
            toast.success(res.EM || 'Xóa hóa đơn thành công');
            handleClose();
            await fetchListInvoice(1);
            setCurrentPage(1);
        } else {
            toast.error(res?.EM || 'Xóa hóa đơn thất bại');
        }
    };

    return (
        <Modal show={show} onHide={handleClose} backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>Xóa hóa đơn</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Bạn có chắc chắn muốn xóa hóa đơn <strong>#{dataDelete?.invoiceID}</strong> của khách hàng <strong>{dataDelete?.customerName}</strong>?
                <br />
                <span className="text-danger">Hành động này không thể hoàn tác.</span>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Hủy
                </Button>
                <Button variant="danger" onClick={() => handleDelete()}>
                    Xóa
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalDeleteInvoice;
