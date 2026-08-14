import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { FaPlus } from 'react-icons/fa';
import { toast } from 'react-toastify';
import { ToastContainer, Toast, Bounce } from "react-toastify";

import { UpdateCustomer, getAllCustomerType } from '../../services/apiService';

import _ from 'lodash';

const ModalUpdateCustomer = (props) => {

    const { show, setShow, dataUpdate } = props;

    const [customerName, setCustomerName] = useState();
    const [phone, setPhone] = useState();
    const [address, setAddress] = useState();
    const [customerTypeID, setCustomerTypeID] = useState();
    const [listCustomerType, setListCustomerType] = useState([]);

    useEffect(() => {
        fetchListCustomerType();
    }, [])

    const fetchListCustomerType = async () => {
        let res = await getAllCustomerType();
        if (res && res.ec === 0) {
            setListCustomerType(res.dt.customerTypes);
        }
    }

    useEffect(() => {
        if (!_.isEmpty(dataUpdate)) {
            setCustomerName(dataUpdate.customerName)
            setAddress(dataUpdate.address);
            setPhone(dataUpdate.phone);
            setCustomerTypeID(dataUpdate.customerTypeID);
        }
    }, [dataUpdate])

    const handleClose = () => {
        setShow(false);
        setCustomerName("");
        setPhone("");
        setAddress("");
        setCustomerTypeID("");
    };

    const handleSubmitCustomer = async () => {
        if (!customerName || customerName === "") {
            toast.error('Invalid Customer Name');
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
        if (!customerTypeID || customerTypeID === "") {
            toast.error('Invalid Customer Type');
            return;
        }

        let res = await UpdateCustomer(dataUpdate.customerID, customerName, phone, address, customerTypeID);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Update customer failed');
            return;
        }
        toast.success(res.EM);
        handleClose();
        await props.fetchListCustomer(1);
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-update-customer'>
                <Modal.Header closeButton>
                    <Modal.Title>Update Customer</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label >Customer Name</label>
                            <input type="text" className="form-control" value={customerName} placeholder="Customer Name" onChange={(event) => setCustomerName(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Address</label>
                            <input type="text" className="form-control" value={address} placeholder="Address" onChange={(event) => setAddress(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Phone</label>
                            <input type="text" className="form-control" value={phone} placeholder="Phone" onChange={(event) => setPhone(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Customer Type</label>
                            <select className="form-control" value={customerTypeID} onChange={(event) => setCustomerTypeID(event.target.value)}>
                                <option value="">-- Select Customer Type --</option>
                                {listCustomerType && listCustomerType.length > 0 &&
                                    listCustomerType.map((item, index) => {
                                        return (
                                            <option key={`option-type-${index}`} value={item.customerTypeID}>{item.typeName}</option>
                                        )
                                    })
                                }
                            </select>
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitCustomer()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalUpdateCustomer;
