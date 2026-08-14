import { useEffect, useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

import { FaPlus } from 'react-icons/fa';
import { toast } from 'react-toastify';
import { UpdateUser, putAssign_RoleBranch } from '../../services/apiService';
import _ from 'lodash';

const ModalUpdateUser = (props) => {

    const { show, setShow, dataUpdate, listRole } = props;

    const handleClose = () => {
        setShow(false);
        setEmail("");
        setUsername("");
        setFullname("");
        setPassword("");
        setImage("");
        setRole("");
        setAddress("");
        setPhone("");
    };

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [username, setUsername] = useState("");
    const [fullname, setFullname] = useState("");
    const [address, setAddress] = useState("");
    const [phone, setPhone] = useState("");
    const [role, setRole] = useState("");

    const [image, setImage] = useState("");
    const [previewImage, setPreviewImage] = useState("");

    useEffect(() => {
        if (!_.isEmpty(dataUpdate)) {
            setEmail(dataUpdate.email);
            setUsername(dataUpdate.userName);
            setFullname(dataUpdate.fullName);
            setAddress(dataUpdate.address);
            setPhone(dataUpdate.phone);
            setRole(dataUpdate.roleID || dataUpdate.roleName || "");

            setImage("");
            setPreviewImage(dataUpdate.image ? `data:image/png;base64,${dataUpdate.image}` : "");
        }
    }, [dataUpdate]);

    const handleUpLoadFile = (event) => {
        if (event.target && event.target.files && event.target.files[0]) {
            setPreviewImage(URL.createObjectURL(event.target.files[0]));
            setImage(event.target.files[0]);
        }
    }

    const validateEmail = (email) => {
        return String(email)
            .toLowerCase()
            .match(
                /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            );
    }

    const handleSubMitUser = async () => {
        const isValidEmail = validateEmail(email);
        if (!isValidEmail) {
            toast.error('Invalid email');
            return;
        }

        let res = await UpdateUser(dataUpdate.id, fullname, phone, address, email);
        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Update user failed');
            return;
        }

        let res_put = await putAssign_RoleBranch(dataUpdate.id, role);
        if (res_put && res_put.ec === 0) {
            toast.success(res.EM + "\n" + res_put.EM);
            handleClose();
            await props.fetchListUserPage(1);
        } else {
            toast.error(res_put?.EM || 'Assign role failed');
        }
    }

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-create-user' backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>Update user</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <form className='row g-3'>
                        <div className="form-group col-md-6">
                            <label >FullName</label>
                            <input type="text" className="form-control" value={fullname} placeholder="FullName" onChange={(event) => setFullname(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Email</label>
                            <input type="email" className="form-control" value={email} placeholder="Email" onChange={(event) => setEmail(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Phone</label>
                            <input type="text" className="form-control" value={phone} placeholder="Phone" onChange={(event) => setPhone(event.target.value)} />
                        </div>
                        <div className="form-group col-md-6">
                            <label >Address</label>
                            <input type="text" className="form-control" value={address} placeholder="Address" onChange={(event) => setAddress(event.target.value)} />
                        </div>
                        <div className="form-group col-md-4">
                            <label>Role</label>
                            <select className="form-control" value={role} onChange={(event) => setRole(event.target.value)}>
                                <option value="">-- Select Role --</option>
                                {listRole && listRole.length > 0 &&
                                    listRole.map((item, index) => (
                                        <option key={`role-${index}`} value={item.roleID}>
                                            {item.roleName}
                                        </option>
                                    ))
                                }
                            </select>
                        </div>
                        <div className='col-md-12'>
                            <label className='form-label label-upload' htmlFor='showInputImage'>
                                <FaPlus />Upload File Image</label>
                            <input type='file' id='showInputImage' onChange={(event) => handleUpLoadFile(event)} hidden />
                        </div>
                        <div className='col-md-12 img-preview'>
                            {previewImage ?
                                <img src={previewImage} />
                                :
                                <span>preview Image</span>
                            }
                        </div>
                    </form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => handleSubMitUser()}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}
export default ModalUpdateUser;
