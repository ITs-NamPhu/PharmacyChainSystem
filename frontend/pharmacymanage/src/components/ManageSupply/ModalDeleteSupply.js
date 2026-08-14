import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { DeleteSupplier } from '../../services/apiService';
import { toast } from 'react-toastify';

const ModalDeleteSupply = (props) => {
    const { show, setShow, dataDelete } = props;

    const handleClose = () => setShow(false);

    const handleSubmitDeleteSupply = async () => {
        let res = await DeleteSupplier(dataDelete.supplierID);
        if (res && res.ec === 0) {
            toast.success(res.EM);
            handleClose();
            props.setCurrentPage(1);
            await props.fetchListSupply(1)
        } else {
            toast.error(res?.EM || 'Delete failed');
        }
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Delete Supplier</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {/* Are you sure delete supplier: <b>{dataDelete && dataDelete.supplierName ? dataDelete.supplierName : ""}</b> !!! */}
                    Hệ thống đang phát triển chức năng này

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Cancel
                    </Button>
                    {/* <Button variant="primary" onClick={() => handleSubmitDeleteSupply()}>
                        Confirm
                    </Button> */}
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalDeleteSupply;
