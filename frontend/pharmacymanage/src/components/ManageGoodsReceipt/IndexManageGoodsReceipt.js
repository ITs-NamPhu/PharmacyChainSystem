import { useState, useEffect } from 'react';

import { MdAddCircle } from "react-icons/md";

import ModalCreateGoodsReceipt from './ModalCreateGoodsReceipt';
import ModalUpdateGoodsReceipt from './ModalUpdateGoodsReceipt';
import ModalDeleteGoodsReceipt from './ModalDeleteGoodsReceipt';
import TableGoodsReceipt from './tableGoodsReceipt';

import './IndexManageGoodsReceipt.scss';

import { getAllGoodsReceiptPag } from '../../services/apiService';

const IndexManageGoodsReceipt = (props) => {

    const [listGoodsReceipt, setListGoodsReceipt] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateGoodsReceipt, setShowModalCreateGoodsReceipt] = useState(false);

    const [showModalUpdateGoodsReceipt, setShowModalUpdateGoodsReceipt] = useState(false);
    const [dataUpdate, setDataUpdate] = useState({});

    const [showModalDeleteGoodsReceipt, setShowModalDeleteGoodsReceipt] = useState(false);
    const [dataDelete, setDataDelete] = useState({});

    useEffect(() => {
        fetchListGoodsReceipt(1)
    }, [])

    const fetchListGoodsReceipt = async (page) => {
        let res = await getAllGoodsReceiptPag(page, limitPage);
        if (res.ec === 0) {
            setListGoodsReceipt(res.dt.goodsReceipts);
            setPageCount(res.dt.TotalPage);
        }
    }

    const handleCreateGoodsReceipt = () => {
        setShowModalCreateGoodsReceipt(!showModalCreateGoodsReceipt);
    }
    const handleUpdateGoodsReceipt = (goodsReceipt) => {
        setShowModalUpdateGoodsReceipt(!showModalUpdateGoodsReceipt);
        setDataUpdate(goodsReceipt);
    }
    const handleDeleteGoodsReceipt = (goodsReceipt) => {
        setShowModalDeleteGoodsReceipt(!showModalDeleteGoodsReceipt);
        setDataDelete(goodsReceipt);
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Management Goods Receipt
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateGoodsReceipt()}>
                        <MdAddCircle /> Add Goods Receipt
                    </button>
                </div>

                <div className='table-container'>
                    <TableGoodsReceipt
                        fetchListGoodsReceipt={fetchListGoodsReceipt}
                        listGoodsReceipt={listGoodsReceipt}
                        handleUpdateGoodsReceipt={handleUpdateGoodsReceipt}
                        handleDeleteGoodsReceipt={handleDeleteGoodsReceipt}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} >
                    </TableGoodsReceipt>
                </div>

                <ModalCreateGoodsReceipt
                    show={showModalCreateGoodsReceipt} setShow={setShowModalCreateGoodsReceipt}
                    fetchListGoodsReceipt={fetchListGoodsReceipt}
                />

                <ModalUpdateGoodsReceipt
                    show={showModalUpdateGoodsReceipt} setShow={setShowModalUpdateGoodsReceipt}
                    fetchListGoodsReceipt={fetchListGoodsReceipt}
                    dataUpdate={dataUpdate}
                />

                <ModalDeleteGoodsReceipt
                    show={showModalDeleteGoodsReceipt} setShow={setShowModalDeleteGoodsReceipt}
                    fetchListGoodsReceipt={fetchListGoodsReceipt}
                    dataDelete={dataDelete}
                    setCurrentPage={setCurrentPage}
                />
            </div>
        </div>
    );
}

export default IndexManageGoodsReceipt;
