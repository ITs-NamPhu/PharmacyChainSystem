import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';
import { UpdateUnit } from '../../services/apiService';
import _ from 'lodash';

const ModalUpdateUnit = (props) => {
    const { show, setShow, dataUpdate } = props;
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

        let res = await UpdateUnit(dataUpdate.unitID, unitName);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Update unit failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListUnit(1);
    }

    useEffect(() => {
        if (!_.isEmpty(dataUpdate)) {
            setUnitName(dataUpdate.unitName)
        }
    }, [dataUpdate])

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-update-unit'>
                <Modal.Header closeButton>
                    <Modal.Title>Update Unit</Modal.Title>
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
export default ModalUpdateUnit;
