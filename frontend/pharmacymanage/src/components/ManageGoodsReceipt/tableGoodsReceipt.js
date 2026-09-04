import ReactPaginate from "react-paginate";
import { getStatusBadge } from '../../utils/statusTicket';

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

    const canApprove = (value) => value.status === "PENDING";
    const canReject = (value) => value.status === "PENDING";
    const canComplete = (value) => value.status === "APPROVED";
    const canUpdate = (value) => value.status === "REJECTED";
    const canDelete = (value) => value.status === "PENDING" || value.status === "REJECTED";

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Số phiếu</td>
                        <td>Nhà cung cấp</td>
                        <td>Người tạo</td>
                        <td>Ngày nhập</td>
                        <td>Tổng tiền</td>
                        <td>Đã trả</td>
                        <td>Ghi chú</td>
                        <td>Trạng thái</td>
                        <td>Thao tác</td>
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
                                    <td>{getStatusBadge(value.status)}</td>
                                    <td>
                                        <button className="btn btn-secondary btn-sm"
                                            onClick={() => props.handleViewGoodsReceipt(value)}>View</button>
                                        {canApprove(value) &&
                                            <button className="btn btn-success btn-sm mx-1"
                                                onClick={() => props.handleApproveGoodsReceipt(value)}>Duyệt</button>}
                                        {canReject(value) &&
                                            <button className="btn btn-danger btn-sm"
                                                onClick={() => props.handleRejectGoodsReceipt(value)}>Từ chối</button>}
                                        {canComplete(value) &&
                                            <button className="btn btn-primary btn-sm mx-1"
                                                onClick={() => props.handleCompleteGoodsReceipt(value)}>Nhập kho</button>}
                                        {canUpdate(value) &&
                                            <button className="btn btn-warning btn-sm mx-1 text-dark"
                                                onClick={() => props.handleUpdateGoodsReceipt(value)}>Sửa phiếu</button>}
                                        {canDelete(value) &&
                                            <button className="btn btn-outline-danger btn-sm mx-1"
                                                onClick={() => props.handleDeleteGoodsReceipt(value)}>Xóa</button>}
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listGoodsReceipt && listGoodsReceipt.length === 0 &&
                        <tr>
                            <td colSpan={10}>"Not found goods receipt"</td>
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
