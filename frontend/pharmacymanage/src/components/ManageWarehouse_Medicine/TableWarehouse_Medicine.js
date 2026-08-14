import ReactPaginate from "react-paginate";

const TableWarehouse_Medicine = (props) => {
    const { listBatch, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListBatch(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    const formatDate = (dateStr) => {
        if (!dateStr) return "";
        const d = new Date(dateStr);
        return d.toLocaleDateString("vi-VN");
    }

    const formatCurrency = (value) => {
        if (value == null) return "";
        return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(value);
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>Batch ID</td>
                        <td>Tên thuốc</td>
                        <td>Đơn vị</td>
                        <td>Đơn giá</td>
                        <td>SL nhập</td>
                        <td>SL tồn</td>
                        <td>NSX</td>
                        <td>HSD</td>
                        <td>Ghi chú</td>
                    </tr>
                </thead>

                <tbody>
                    {listBatch && listBatch.length > 0 &&
                        listBatch.map((value, index) => {
                            return (
                                <tr key={`table-batch-${index}`}>
                                    <td>{value.batchID}</td>
                                    <td>{value.medicineName}</td>
                                    <td>{value.unitName}</td>
                                    <td>{formatCurrency(value.unitCost)}</td>
                                    <td>{value.quantityReceived}</td>
                                    <td>{value.quantityInStock}</td>
                                    <td>{formatDate(value.manufactureDate)}</td>
                                    <td>{formatDate(value.expiryDate)}</td>
                                    <td>{value.note}</td>
                                </tr>
                            )
                        })
                    }
                    {listBatch && listBatch.length === 0 &&
                        <tr>
                            <td colSpan={9}>Không có lô thuốc nào trong kho</td>
                        </tr>
                    }
                </tbody>
            </table>
            <div className="user-paginate">
                <ReactPaginate
                    nextLabel="next >"
                    onPageChange={handlePageClick}
                    pageRangeDisplayed={3}
                    marginPagesDisplayed={2}
                    pageCount={pageCount}
                    previousLabel="< previous"
                    pageClassName="page-item"
                    pageLinkClassName="page-link"
                    previousClassName="page-item"
                    previousLinkClassName="page-link"
                    nextClassName="page-item"
                    nextLinkClassName="page-link"
                    breakLabel="..."
                    breakClassName="page-item"
                    breakLinkClassName="page-link"
                    containerClassName="pagination"
                    activeClassName="active"
                    renderOnZeroPageCount={null}
                    forcePage={props.currentPage - 1}
                />
            </div>
        </>
    );
}

export default TableWarehouse_Medicine;
