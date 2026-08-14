import { useState, useEffect } from 'react';
import { MdAddCircle } from "react-icons/md";
import ModalCreateSupply from './ModalCreateSupply';
import ModalUpdateSupply from './ModalUpdateSupply';
import ModalDeleteSupply from './ModalDeleteSupply';
import TableSupply from './tableSupply';
import './IndexManageSupply.scss';
import { getAllSupplierPag } from '../../services/apiService'

const IndexManageSupply = (props) => {
    const [listSupply, setListSupply] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateSupply, setShowModalCreateSupply] = useState(false);

    const [showModalUpdateSupply, setShowModalUpdateSupply] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteSupply, setShowModalDeleteSupply] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListSupply(1)
    }, [])

    const fetchListSupply = async (page) => {
        let res = await getAllSupplierPag(page, limitPage);
        if (res.ec === 0) {
            setListSupply(res.dt.suppliers);
            setPageCount(res.dt.TotalPage);
        }
    }

    const handleCreateSupply = () => {
        setShowModalCreateSupply(!showModalCreateSupply);
    }
    const handleUpdateSupply = (supply) => {
        setShowModalUpdateSupply(!showModalUpdateSupply);
        setDataUpdate(supply);
    }
    const handleDeleteSupply = (supply) => {
        setShowModalDeleteSupply(!showModalDeleteSupply);
        setDataDelete(supply);
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Management Supplier
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateSupply()}>
                        <MdAddCircle /> Add Supplier
                    </button>
                </div>
                <div className='table-container'>
                    <TableSupply
                        fetchListSupply={fetchListSupply}
                        listSupply={listSupply}
                        handleUpdateSupply={handleUpdateSupply}
                        handleDeleteSupply={handleDeleteSupply}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateSupply
                    show={showModalCreateSupply} setShow={setShowModalCreateSupply}
                    fetchListSupply={fetchListSupply}
                />
                <ModalUpdateSupply
                    show={showModalUpdateSupply} setShow={setShowModalUpdateSupply}
                    fetchListSupply={fetchListSupply}
                    dataUpdate={dataUpdate}
                />
                <ModalDeleteSupply
                    show={showModalDeleteSupply} setShow={setShowModalDeleteSupply}
                    fetchListSupply={fetchListSupply}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
}
export default IndexManageSupply;
