import { useState } from 'react';
import { useEffect } from 'react';
import { useSelector } from 'react-redux';

import { MdAddCircle } from "react-icons/md";

import {
    getUserbyBranch,
    getAllRole,
} from '../../services/apiService';

import './IndexManageUser.scss'
import ModalCreateUser from './ModalCreateUser';
import ModalUpdateUser from './ModalUpdateUser';
import ModalDeleteUser from './ModalDeleteUser';
import TableUser from './tableUser';


const IndexManageUser = (props) => {
    const currentBranchId = useSelector(state => state.user.account.currentBranchId)

    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [listUser, setListUser] = useState();

    const [listRole, setListRole] = useState();

    const [showModalCreateUser, setShowModalCreateUser] = useState(false);

    const [showModalUpdateUser, setShowModalUpdateUser] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteUser, setShowModalDeleteUser] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListRole();
    }, [])

    useEffect(() => {
        if (currentBranchId) {
            fetchListUserPage(1);
        }
    }, [currentBranchId])

    const fetchListUserPage = async (page) => {
        let res = await getUserbyBranch(page, limitPage);
        if (res.ec === 0) {
            setListUser(res.dt.users);
            setPageCount(res.dt.TotalPage);
        }
    }

    const fetchListRole = async () => {
        let res = await getAllRole();
        if (res.ec === 0)
            setListRole(res.dt);
    }

    const handleCreateUser = () => {
        setShowModalCreateUser(!showModalCreateUser);
    }
    const handleUpdateUser = (user) => {
        setShowModalUpdateUser(!showModalUpdateUser);
        setDataUpdate(user);
    }
    const handleDeleteUser = (user) => {
        setShowModalDeleteUser(!showModalDeleteUser);
        setDataDelete(user);
    }

    return (
        <div className='manage-user-container'>
            <div className="title">
                Management User
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateUser()}>
                        <MdAddCircle /> Add User
                    </button>
                </div >
                <div className='table-users-container'>
                    <TableUser
                        changePage={fetchListUserPage}
                        listUser={listUser}
                        handleUpdateUser={handleUpdateUser}
                        handleDeleteUser={handleDeleteUser}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage}
                    >
                    </TableUser>
                </div>

                <ModalCreateUser
                    show={showModalCreateUser} setShow={setShowModalCreateUser}
                    fetchListUserPage={fetchListUserPage}
                    listRole={listRole}
                />

                <ModalUpdateUser
                    show={showModalUpdateUser} setShow={setShowModalUpdateUser}
                    fetchListUserPage={fetchListUserPage}
                    listRole={listRole}
                    dataUpdate={dataUpdate}
                />

                <ModalDeleteUser show={showModalDeleteUser} setShow={setShowModalDeleteUser}
                    fetchListUserPage={fetchListUserPage}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div >
        </div >
    );

}
export default IndexManageUser;
