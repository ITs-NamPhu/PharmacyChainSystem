import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { DeleteUnit } from '../../services/apiService';
import { toast } from 'react-toastify';

const ModalDeleteUnit = (props) => {
    const { show, setShow, dataDelete } = props;

    const handleClose = () => setShow(false);

    const handleSubmitDeleteUnit = async () => {
        let res = await DeleteUnit(dataDelete.unitID);
        if (res && res.ec === 0) {
            toast.success(res.EM);
            handleClose();
            props.setCurrentPage(1);
            await props.fetchListUnit(1)
        } else {
            toast.error(res?.EM || 'Delete failed');
        }
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Delete Unit</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Hệ thống đang phát triển chức năng này

                    {/* Are you sure delete unit: <b>{dataDelete && dataDelete.unitName ? dataDelete.unitName : ""}</b> !!! */}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Cancel
                    </Button>
                    {/* <Button variant="primary" onClick={() => handleSubmitDeleteUnit()}>
                        Confirm
                    </Button> */}
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalDeleteUnit;
