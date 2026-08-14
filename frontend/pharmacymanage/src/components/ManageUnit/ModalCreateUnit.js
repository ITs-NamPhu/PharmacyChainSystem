import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';
import { CreateUnit } from '../../services/apiService';

const ModalCreateUnit = (props) => {
    const { show, setShow } = props;
    const [unitName, setUnitName] = useState();

    const handleClose = () => {
        setShow(false);
        setUnitName("");
    };

    const handleSubmitUnit = async () => {
        if (!unitName || unitName === "") {
            toast.error('Invalid Unit Name');
            return;
        }

        let res = await CreateUnit(unitName);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Create unit failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListUnit(1);
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-unit'>
                <Modal.Header closeButton>
                    <Modal.Title>Create Unit</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label>Unit Name</label>
                            <input type="text" className="form-control" value={unitName} placeholder="Unit Name" onChange={(event) => setUnitName(event.target.value)} />
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitUnit()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalCreateUnit;
