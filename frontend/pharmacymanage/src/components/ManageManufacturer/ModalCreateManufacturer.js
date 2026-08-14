import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';
import { CreateManufacturer } from '../../services/apiService';

const ModalCreateManufacturer = (props) => {
    const { show, setShow } = props;
    const [manufacturerName, setManufacturerName] = useState();

    const handleClose = () => {
        setShow(false);
        setManufacturerName("");
    };

    const handleSubmitManufacturer = async () => {
        if (!manufacturerName || manufacturerName === "") {
            toast.error('Invalid Manufacturer Name');
            return;
        }

        let res = await CreateManufacturer(manufacturerName);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Create manufacturer failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListManufacturer(1);
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-manufacturer'>
                <Modal.Header closeButton>
                    <Modal.Title>Create Manufacturer</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label>Manufacturer Name</label>
                            <input type="text" className="form-control" value={manufacturerName} placeholder="Manufacturer Name" onChange={(event) => setManufacturerName(event.target.value)} />
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitManufacturer()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalCreateManufacturer;
