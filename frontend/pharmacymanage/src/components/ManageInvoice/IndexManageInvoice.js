import { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { MdAddCircle } from "react-icons/md";

import ModalCreateInvoice from './ModalCreateInvoice';
import ModalUpdateInvoice from './ModalUpdateInvoice';
import ModalDeleteInvoice from './ModalDeleteInvoice';
import TableInvoice from './tableInvoice';

import './IndexManageInvoice.scss';

import { getAllInvoicePag } from '../../services/apiService';

const IndexManageInvoice = () => {

    const [listInvoice, setListInvoice] = useState([]);

    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateInvoice, setShowModalCreateInvoice] = useState(false);

    const [showModalUpdateInvoice, setShowModalUpdateInvoice] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteInvoice, setShowModalDeleteInvoice] = useState(false);
    const [dataDelete, setDataDelete] = useState({});


    useEffect(() => {
        fetchListInvoice(1);
    }, []);

    const fetchListInvoice = async (page) => {
        let res = await getAllInvoicePag(page, limitPage);
        if (res.ec === 0) {
            setListInvoice(res.dt.invoices);
            setPageCount(res.dt.totalPage);
        }
    };

    const handleCreateInvoice = () => {
        setShowModalCreateInvoice(!showModalCreateInvoice);
    };
    const handleUpdateInvoice = (invoice) => {
        setShowModalUpdateInvoice(!showModalUpdateInvoice);
        setDataUpdate(invoice);
    };
    const handleDeleteInvoice = (invoice) => {
        setShowModalDeleteInvoice(!showModalDeleteInvoice);
        setDataDelete(invoice);
    };

    return (
        <div className='manage-container'>
            <div className="title">
                Quản lý hóa đơn bán hàng
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateInvoice()}>
                        <MdAddCircle /> Thêm hóa đơn
                    </button>
                </div>
                <div className='table-container'>
                    <TableInvoice
                        fetchListInvoice={fetchListInvoice}
                        listInvoice={listInvoice}
                        handleUpdateInvoice={handleUpdateInvoice}
                        handleDeleteInvoice={handleDeleteInvoice}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateInvoice
                    show={showModalCreateInvoice} setShow={setShowModalCreateInvoice}
                    fetchListInvoice={fetchListInvoice}
                />
                <ModalUpdateInvoice
                    show={showModalUpdateInvoice} setShow={setShowModalUpdateInvoice}
                    fetchListInvoice={fetchListInvoice}
                    dataUpdate={dataUpdate}
                />
                <ModalDeleteInvoice
                    show={showModalDeleteInvoice} setShow={setShowModalDeleteInvoice}
                    fetchListInvoice={fetchListInvoice}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
};
export default IndexManageInvoice;
