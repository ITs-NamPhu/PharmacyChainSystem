import { useState, useEffect } from 'react';
import { MdAddCircle } from "react-icons/md";

import ModalCreateMedicine from './ModalCreateMedicine';
import ModalUpdateMedicine from './ModalUpdateMedicine';
import ModalDeleteMedicine from './ModalDeleteMedicine';
import TableMedicine from './tableMedicine';

import './IndexManageMedicine.scss';

import { getAllMedicinePag } from '../../services/apiService';

const IndexManageMedicine = () => {

    const [listMedicine, setListMedicine] = useState([]);
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateMedicine, setShowModalCreateMedicine] = useState(false);

    const [showModalUpdateMedicine, setShowModalUpdateMedicine] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteMedicine, setShowModalDeleteMedicine] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListMedicine(1);
    }, []);

    const fetchListMedicine = async (page) => {
        let res = await getAllMedicinePag(page, limitPage);
        if (res.ec === 0) {
            setListMedicine(res.dt.medicines);
            setPageCount(res.dt.totalPage);
        }
    };

    const handleCreateMedicine = () => {
        setShowModalCreateMedicine(!showModalCreateMedicine);
    };
    const handleUpdateMedicine = (medicine) => {
        setShowModalUpdateMedicine(!showModalUpdateMedicine);
        setDataUpdate(medicine);
    };
    const handleDeleteMedicine = (medicine) => {
        setShowModalDeleteMedicine(!showModalDeleteMedicine);
        setDataDelete(medicine);
    };

    return (
        <div className='manage-container'>
            <div className="title">
                Management Medicine
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateMedicine()}>
                        <MdAddCircle /> Add Medicine
                    </button>
                </div>
                <div className='table-container'>
                    <TableMedicine
                        fetchListMedicine={fetchListMedicine}
                        listMedicine={listMedicine}
                        handleUpdateMedicine={handleUpdateMedicine}
                        handleDeleteMedicine={handleDeleteMedicine}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateMedicine
                    show={showModalCreateMedicine} setShow={setShowModalCreateMedicine}
                    fetchListMedicine={fetchListMedicine}
                />
                <ModalUpdateMedicine
                    show={showModalUpdateMedicine} setShow={setShowModalUpdateMedicine}
                    fetchListMedicine={fetchListMedicine}
                    dataUpdate={dataUpdate}
                />
                <ModalDeleteMedicine
                    show={showModalDeleteMedicine} setShow={setShowModalDeleteMedicine}
                    fetchListMedicine={fetchListMedicine}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
};
export default IndexManageMedicine;
