import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { DeleteMedicine } from '../../services/apiService';
import { toast } from 'react-toastify';

const ModalDeleteMedicine = (props) => {
    const { show, setShow, dataDelete } = props;

    const handleClose = () => setShow(false);

    const handleSubmitDeleteMedicine = async () => {
        let res = await DeleteMedicine(dataDelete.medicineID);
        if (res && res.ec === 0) {
            toast.success(res.EM);
            handleClose();
            props.setCurrentPage(1);
            await props.fetchListMedicine(1);
        } else {
            toast.error(res?.EM || 'Xóa thuốc thất bại');
        }
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Xác nhận xóa thuốc</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Bạn có chắc chắn muốn xóa thuốc: <b>{dataDelete && dataDelete.medicineName ? dataDelete.medicineName : ""}</b> ?
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Hủy
                    </Button>
                    <Button variant="danger" onClick={() => handleSubmitDeleteMedicine()}>
                        Xóa
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalDeleteMedicine;
