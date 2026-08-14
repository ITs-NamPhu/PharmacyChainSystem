import { useState, useEffect } from 'react';
import { MdAddCircle } from "react-icons/md";
import ModalCreateCustomer from './ModalCreateCustomer';
import ModalUpdateCustomer from './ModalUpdateCustomer';
import ModalDeleteCustomer from './ModalDeleteCustomer';
import TableCustomer from './tableCustomer';
import './IndexManageCustomer.scss';
import { getAllCustomerPag } from '../../services/apiService'

const IndexManageCustomer = (props) => {
    const [listCustomer, setListCustomer] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateCustomer, setShowModalCreateCustomer] = useState(false);

    const [showModalUpdateCustomer, setShowModalUpdateCustomer] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteCustomer, setShowModalDeleteCustomer] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListCustomer(1)
    }, [])

    const fetchListCustomer = async (page) => {
        let res = await getAllCustomerPag(page, limitPage);
        if (res.ec === 0) {
            setListCustomer(res.dt.customers);
            setPageCount(res.dt.TotalPage);
        }
    }

    const handleCreateCustomer = () => {
        setShowModalCreateCustomer(!showModalCreateCustomer);
    }
    const handleUpdateCustomer = (customer) => {
        setShowModalUpdateCustomer(!showModalUpdateCustomer);
        setDataUpdate(customer);
    }
    const handleDeleteCustomer = (customer) => {
        setShowModalDeleteCustomer(!showModalDeleteCustomer);
        setDataDelete(customer);
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Management Customer
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateCustomer()}>
                        <MdAddCircle /> Add Customer
                    </button>
                </div>
                <div className='table-container'>
                    <TableCustomer
                        fetchListCustomer={fetchListCustomer}
                        listCustomer={listCustomer}
                        handleUpdateCustomer={handleUpdateCustomer}
                        handleDeleteCustomer={handleDeleteCustomer}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateCustomer
                    show={showModalCreateCustomer} setShow={setShowModalCreateCustomer}
                    fetchListCustomer={fetchListCustomer}
                />
                <ModalUpdateCustomer
                    show={showModalUpdateCustomer} setShow={setShowModalUpdateCustomer}
                    fetchListCustomer={fetchListCustomer}
                    dataUpdate={dataUpdate}
                />
                <ModalDeleteCustomer
                    show={showModalDeleteCustomer} setShow={setShowModalDeleteCustomer}
                    fetchListCustomer={fetchListCustomer}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
}
export default IndexManageCustomer;
