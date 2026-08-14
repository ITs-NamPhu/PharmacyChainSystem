import { useState, useEffect } from 'react';

import { MdAddCircle } from "react-icons/md";

import ModalCreateBranch from './ModalCreateBranch';
import ModalUpdateBranch from './ModalUpdateBranch';
import ModalDeleteBranch from './ModalDeleteBranch';
import TableBranch from './tableBranch';

import './IndexManageBranch.scss';

import { getAllBranchPag } from '../../services/apiService';
const IndexManageBranch = (props) => {

    const [listBranch, setListBranch] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateBranch, setShowModalCreateBranch] = useState(false);

    const [showModalUpdateUBranch, setShowModalUpdateBranch] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteBranch, setShowModalDeleteBranch] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListBranch(1)
    }, [])

    const fetchListBranch = async (page) => {
        let res = await getAllBranchPag(page, limitPage);
        console.log('res_branch', res)
        if (res.ec === 0) {
            setListBranch(res.dt.branches);
            setPageCount(res.dt.TotalPage);
        }
    }

    const handleCreateBranch = () => {
        setShowModalCreateBranch(!showModalCreateBranch);
    }
    const handleUpdateBranch = (branch) => {
        setShowModalUpdateBranch(!showModalUpdateUBranch);
        setDataUpdate(branch);
    }
    const handleDeleteBranch = (branch) => {
        setShowModalDeleteBranch(!showModalDeleteBranch);
        setDataDelete(branch);
    }

    return (
        <div className='manage-user-container'>
            <div className="title">
                Management Branch
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateBranch()}>
                        <MdAddCircle /> Add User
                    </button>
                </div>

                <div className='table-container'>
                    <TableBranch
                        fetchListBranch={fetchListBranch}
                        listBranch={listBranch}
                        handleUpdateBranch={handleUpdateBranch}
                        handleDeleteBranch={handleDeleteBranch}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} >
                    </TableBranch>
                </div>

                <ModalCreateBranch
                    show={showModalCreateBranch} setShow={setShowModalCreateBranch}
                    fetchListBranch={fetchListBranch}
                />

                <ModalUpdateBranch
                    show={showModalUpdateUBranch} setShow={setShowModalUpdateBranch}
                    fetchListBranch={fetchListBranch}
                    dataUpdate={dataUpdate}
                />

                <ModalDeleteBranch
                    show={showModalDeleteBranch} setShow={setShowModalDeleteBranch}
                    fetchListBranch={fetchListBranch}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
}

export default IndexManageBranch;