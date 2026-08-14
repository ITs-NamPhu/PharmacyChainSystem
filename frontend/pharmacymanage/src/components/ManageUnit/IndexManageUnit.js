import { useState, useEffect } from 'react';
import { MdAddCircle } from "react-icons/md";
import ModalCreateUnit from './ModalCreateUnit';
import ModalUpdateUnit from './ModalUpdateUnit';
import ModalDeleteUnit from './ModalDeleteUnit';
import TableUnit from './tableUnit';
import './IndexManageUnit.scss';
import { getAllUnitPag } from '../../services/apiService'

const IndexManageUnit = (props) => {
    const [listUnit, setListUnit] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateUnit, setShowModalCreateUnit] = useState(false);

    const [showModalUpdateUnit, setShowModalUpdateUnit] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteUnit, setShowModalDeleteUnit] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListUnit(1)
    }, [])

    const fetchListUnit = async (page) => {
        let res = await getAllUnitPag(page, limitPage);
        if (res.ec === 0) {
            setListUnit(res.dt.units);
            setPageCount(res.dt.TotalPage);
        }
    }

    const handleCreateUnit = () => {
        setShowModalCreateUnit(!showModalCreateUnit);
    }
    const handleUpdateUnit = (unit) => {
        setShowModalUpdateUnit(!showModalUpdateUnit);
        setDataUpdate(unit);
    }
    const handleDeleteUnit = (unit) => {
        setShowModalDeleteUnit(!showModalDeleteUnit);
        setDataDelete(unit);
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Management Unit
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateUnit()}>
                        <MdAddCircle /> Add Unit
                    </button>
                </div>
                <div className='table-container'>
                    <TableUnit
                        fetchListUnit={fetchListUnit}
                        listUnit={listUnit}
                        handleUpdateUnit={handleUpdateUnit}
                        handleDeleteUnit={handleDeleteUnit}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateUnit
                    show={showModalCreateUnit} setShow={setShowModalCreateUnit}
                    fetchListUnit={fetchListUnit}
                />
                <ModalUpdateUnit
                    show={showModalUpdateUnit} setShow={setShowModalUpdateUnit}
                    fetchListUnit={fetchListUnit}
                    dataUpdate={dataUpdate}
                />
                <ModalDeleteUnit
                    show={showModalDeleteUnit} setShow={setShowModalDeleteUnit}
                    fetchListUnit={fetchListUnit}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
}
export default IndexManageUnit;
