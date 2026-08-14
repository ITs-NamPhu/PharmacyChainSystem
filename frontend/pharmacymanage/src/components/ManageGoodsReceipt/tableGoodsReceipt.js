import ReactPaginate from "react-paginate";

const TableGoodsReceipt = (props) => {
    const { listGoodsReceipt, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListGoodsReceipt(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    }

    const formatCurrency = (amount) => {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Receipt Number</td>
                        <td>Supplier</td>
                        <td>User</td>
                        <td>Date</td>
                        <td>Total Amount</td>
                        <td>Paid Amount</td>
                        <td>Note</td>
                        <td>Actions</td>
                    </tr>
                </thead>

                <tbody>
                    {listGoodsReceipt && listGoodsReceipt.length > 0 &&
                        listGoodsReceipt.map((value, index) => {
                            return (
                                <tr key={`table-goods-receipt-${index}`}>
                                    <td>{value.goodsReceiptID}</td>
                                    <td>{value.receiptNumber}</td>
                                    <td>{value.supplierName}</td>
                                    <td>{value.userName}</td>
                                    <td>{formatDate(value.receiptDate)}</td>
                                    <td>{formatCurrency(value.totalAmount)}</td>
                                    <td>{formatCurrency(value.paidAmount)}</td>
                                    <td>{value.note}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateGoodsReceipt(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteGoodsReceipt(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listGoodsReceipt && listGoodsReceipt.length === 0 &&
                        <tr>
                            <td colSpan={9}>"Not found goods receipt"</td>
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

export default TableGoodsReceipt;
