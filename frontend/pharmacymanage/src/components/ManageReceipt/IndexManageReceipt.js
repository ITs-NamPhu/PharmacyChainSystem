import { useState, useEffect } from 'react';
import { MdAddCircle } from "react-icons/md";
import ModalCreateReceipt from './ModalCreateReceipt';
import ModalReceiptDetail from './ModalReceiptDetail';
import TableReceipt from './tableReceipt';
import './IndexManageReceipt.scss';
import { getAllReceiptPag } from '../../services/apiService';

const IndexManageReceipt = (props) => {
    const [listReceipt, setListReceipt] = useState();
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const [showModalCreateReceipt, setShowModalCreateReceipt] = useState(false);

    const [showModalDetailReceipt, setShowModalDetailReceipt] = useState(false);
    const [dataDetail, setDataDetail] = useState({});

    useEffect(() => {
        fetchListReceipt(1)
    }, [])

    const fetchListReceipt = async (page) => {
        let res = await getAllReceiptPag(page, limitPage);
        if (res.ec === 0) {
            setListReceipt(res.dt.items);
            setPageCount(res.dt.totalPage);
        }
    }

    const handleCreateReceipt = () => {
        setShowModalCreateReceipt(!showModalCreateReceipt);
    }

    const handleViewDetail = (receipt) => {
        setDataDetail(receipt);
        setShowModalDetailReceipt(true);
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Quản lý phiếu thu
            </div>
            <div className='user-content'>
                <div className='btn-add-new'>
                    <button className='btn btn-primary' onClick={() => handleCreateReceipt()}>
                        <MdAddCircle /> Thêm phiếu thu
                    </button>
                </div>
                <div className='table-container'>
                    <TableReceipt
                        fetchListReceipt={fetchListReceipt}
                        listReceipt={listReceipt}
                        handleViewDetail={handleViewDetail}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
                <ModalCreateReceipt
                    show={showModalCreateReceipt} setShow={setShowModalCreateReceipt}
                    fetchListReceipt={fetchListReceipt}
                    handleViewDetail={handleViewDetail}
                />
                <ModalReceiptDetail
                    show={showModalDetailReceipt} setShow={setShowModalDetailReceipt}
                    data={dataDetail}
                />
            </div>
        </div>
    );
}
export default IndexManageReceipt;