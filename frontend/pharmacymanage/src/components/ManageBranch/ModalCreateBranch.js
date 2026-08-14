import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { FaPlus } from 'react-icons/fa';
import { toast } from 'react-toastify';
import { ToastContainer, Toast, Bounce } from "react-toastify";

import { CreateBranch } from '../../services/apiService';

const ModalCreateBranch = (props) => {

    const { show, setShow } = props;

    const [branchName, setBranchName] = useState();
    const [phone, setPhone] = useState();
    const [address, setAddress] = useState();

    const handleClose = () => {
        setShow(false);
        setAddress("");
        setPhone("");
        setBranchName("");
    };

    const handleSubmitBranch = async () => {
        if (!branchName || branchName === "") {
            toast.error('Invalid Branch Name');
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

        let res = await CreateBranch(branchName, phone, address);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Create branch failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListBranch(1);
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-branch'>
                <Modal.Header closeButton>
                    <Modal.Title>Create Branch</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label >Branch Name</label>
                            <input type="text" className="form-control" value={branchName} placeholder="Branch Name" onChange={(event) => setBranchName(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Address</label>
                            <input type="password" className="form-control" value={address} placeholder="Address" onChange={(event) => setAddress(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Phone</label>
                            <input type="text" className="form-control" value={phone} placeholder="Phone" onChange={(event) => setPhone(event.target.value)} />
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitBranch()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalCreateBranch;