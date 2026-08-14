import { useState, useEffect } from 'react';
import TableWarehouse_Medicine from './TableWarehouse_Medicine';
import './IndexManageWarehouse_Medicine.scss';
import { getAllBatchInWarehouse } from '../../services/apiService'

const IndexManageWarehouse_Medicine = (props) => {
    const [listBatch, setListBatch] = useState()
    const [limitPage, setLimitPage] = useState(10);
    const [pageCount, setPageCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        fetchListBatch(1)
    }, [])

    const fetchListBatch = async (page) => {
        let res = await getAllBatchInWarehouse(page, limitPage);
        if (res.ec === 0) {
            setListBatch(res.dt.batches);
            setPageCount(res.dt.totalPage);
        }
    }

    return (
        <div className='manage-container'>
            <div className="title">
                Kho thuốc
            </div>
            <div className='user-content'>
                <div className='table-container'>
                    <TableWarehouse_Medicine
                        fetchListBatch={fetchListBatch}
                        listBatch={listBatch}
                        pageCount={pageCount}
                        currentPage={currentPage}
                        setCurrentPage={setCurrentPage} />
                </div>
            </div>
        </div>
    );
}
export default IndexManageWarehouse_Medicine;
