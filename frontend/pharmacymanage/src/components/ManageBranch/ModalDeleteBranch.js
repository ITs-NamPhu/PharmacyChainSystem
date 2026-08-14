import ModalBody from 'react-bootstrap/esm/ModalBody';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

const ModalDeleteBranch = (props) => {
    const { show, setShow } = props;

    const handleClose = () => {
        setShow(false);
    };

    return (
        <Modal show={show} onHide={handleClose} size="xl" className='modal-delete-branch'>
            <Modal.Header closeButton>
                <Modal.Title>
                    Delete Branch
                </Modal.Title>
            </Modal.Header>
            <ModalBody>
                Hệ thống đang phát triển chức năng này

            </ModalBody>

            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Close
                </Button>
                {/* <Button variant="primary" onClick={() => handleSubmitBranch()}>
                    Save Changes
                </Button> */}
            </Modal.Footer>
        </Modal>
    );
}

export default ModalDeleteBranch;