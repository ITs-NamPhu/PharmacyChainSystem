import ReactPaginate from "react-paginate";

const TableInvoice = (props) => {
    const { listInvoice, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListInvoice(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1);
    };

    const formatPrice = (price) => {
        if (price == null) return '';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        return d.toLocaleDateString('vi-VN');
    };

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Khách hàng</td>
                        <td>Tổng tiền</td>
                        <td>Đã thanh toán</td>
                        <td>Ngày tạo</td>
                        <td>Người tạo</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listInvoice && listInvoice.length > 0 &&
                        listInvoice.map((value, index) => {
                            return (
                                <tr key={`table-invoice-${index}`}>
                                    <td>{value.invoiceID}</td>
                                    <td>{value.customerName}</td>
                                    <td>{formatPrice(value.totalAmount)}</td>
                                    <td>{formatPrice(value.paidAmount)}</td>
                                    <td>{formatDate(value.createdAt)}</td>
                                    <td>{value.userName}</td>
                                    <td>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateInvoice(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteInvoice(value)}>Delete</button>
                                    </td>
                                </tr>
                            );
                        })
                    }
                    {listInvoice && listInvoice.length === 0 &&
                        <tr>
                            <td colSpan={7}>"Not found invoice"</td>
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
};

export default TableInvoice;
