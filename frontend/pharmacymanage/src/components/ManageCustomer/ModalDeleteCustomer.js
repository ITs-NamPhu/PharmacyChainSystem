import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { DeleteCustomer } from '../../services/apiService';
import { toast } from 'react-toastify';

const ModalDeleteCustomer = (props) => {
    const { show, setShow, dataDelete } = props;

    const handleClose = () => setShow(false);

    const handleSubmitDeleteCustomer = async () => {
        let res = await DeleteCustomer(dataDelete.customerID);
        if (res && res.ec === 0) {
            toast.success(res.EM);
            handleClose();
            props.setCurrentPage(1);
            await props.fetchListCustomer(1)
        } else {
            toast.error(res?.EM || 'Delete failed');
        }
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Delete Customer</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {/* Are you sure delete customer: <b>{dataDelete && dataDelete.customerName ? dataDelete.customerName : ""}</b> !!! */}
                    Hệ thống đang phát triển chức năng này

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Cancel
                    </Button>
                    {/* <Button variant="primary" onClick={() => handleSubmitDeleteCustomer()}>
                        Confirm
                    </Button> */}
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalDeleteCustomer;
