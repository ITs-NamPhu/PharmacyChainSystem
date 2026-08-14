import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';
import { CreateSupplier } from '../../services/apiService';

const ModalCreateSupply = (props) => {
    const { show, setShow } = props;

    const [supplierName, setSupplierName] = useState();
    const [phone, setPhone] = useState();
    const [email, setEmail] = useState();
    const [address, setAddress] = useState();

    const handleClose = () => {
        setShow(false);
        setSupplierName("");
        setPhone("");
        setEmail("");
        setAddress("");
    };

    const handleSubmitSupply = async () => {
        if (!supplierName || supplierName === "") {
            toast.error('Invalid Supplier Name');
            return;
        }
        if (!address || address === "") {
            toast.error('Invalid Address');
            return;
        }
        if (!phone || !/^\d{10}$/.test(phone)) {
            toast.error('Invalid Phone');
            return;
        }
        if (!email || email === "") {
            toast.error('Invalid Email');
            return;
        }

        let res = await CreateSupplier(supplierName, phone, email, address);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Create supplier failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListSupply(1);
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-supply'>
                <Modal.Header closeButton>
                    <Modal.Title>Create Supplier</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label>Supplier Name</label>
                            <input type="text" className="form-control" value={supplierName} placeholder="Supplier Name" onChange={(event) => setSupplierName(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label>Phone</label>
                            <input type="text" className="form-control" value={phone} placeholder="Phone" onChange={(event) => setPhone(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label>Email</label>
                            <input type="text" className="form-control" value={email} placeholder="Email" onChange={(event) => setEmail(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label>Address</label>
                            <input type="text" className="form-control" value={address} placeholder="Address" onChange={(event) => setAddress(event.target.value)} />
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitSupply()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalCreateSupply;
