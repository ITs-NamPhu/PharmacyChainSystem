import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { DeleteManufacturer } from '../../services/apiService';
import { toast } from 'react-toastify';

const ModalDeleteManufacturer = (props) => {
    const { show, setShow, dataDelete } = props;

    const handleClose = () => setShow(false);

    const handleSubmitDeleteManufacturer = async () => {
        let res = await DeleteManufacturer(dataDelete.manufacturerID);
        if (res && res.ec === 0) {
            toast.success(res.EM);
            handleClose();
            props.setCurrentPage(1);
            await props.fetchListManufacturer(1)
        } else {
            toast.error(res?.EM || 'Delete failed');
        }
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Delete Manufacturer</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {/* Are you sure delete manufacturer: <b>{dataDelete && dataDelete.manufacturerName ? dataDelete.manufacturerName : ""}</b> !!! */}
                    Hệ thống đang phát triển chức năng này

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Cancel
                    </Button>
                    {/* <Button variant="primary" onClick={() => handleSubmitDeleteManufacturer()}>
                        Confirm
                    </Button> */}
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalDeleteManufacturer;
