import ReactPaginate from "react-paginate";
import { MdRemoveRedEye } from "react-icons/md";

const TableReceipt = (props) => {
    const { listReceipt, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListReceipt(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1);
    };

    const formatPrice = (price) => {
        if (price == null) return '0';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        return d.toLocaleString('vi-VN');
    };

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Khách hàng</td>
                        <td>Tổng tiền thu</td>
                        <td>Đã phân bổ</td>
                        <td>Vào ví</td>
                        <td>Ngày tạo</td>
                        <td>Người lập</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listReceipt && listReceipt.length > 0 &&
                        listReceipt.map((value, index) => {
                            return (
                                <tr key={`table-receipt-${index}`}>
                                    <td>{value.receiptID}</td>
                                    <td>{value.customerName}</td>
                                    <td>{formatPrice(value.totalAmount)}</td>
                                    <td>{formatPrice(value.amountApplied)}</td>
                                    <td>{formatPrice(value.walletCredit)}</td>
                                    <td>{formatDate(value.createdDate)}</td>
                                    <td>{value.userName}</td>
                                    <td>
                                        <button className="btn btn-primary btn-sm" onClick={() => props.handleViewDetail(value)}>
                                            <MdRemoveRedEye /> Chi tiết
                                        </button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listReceipt && listReceipt.length === 0 &&
                        <tr>
                            <td colSpan={8}>"Not found receipt"</td>
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

export default TableReceipt;