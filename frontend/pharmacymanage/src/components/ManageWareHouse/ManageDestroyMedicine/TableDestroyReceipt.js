import ReactPaginate from "react-paginate";
import { getStatusBadge } from '../../../utils/statusTicket';

const TableDestroyReceipt = (props) => {
    const { listDestroyReceipt, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchDestroyReceipt(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    const formatDate = (dateStr) => {
        if (!dateStr) return "";
        const d = new Date(dateStr);
        return d.toLocaleDateString("vi-VN") + " " + d.toLocaleTimeString("vi-VN");
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
                        <td>Mã phiếu kiểm kê</td>
                        <td>Kho</td>
                        <td>Người tạo</td>
                        <td>Ngày tạo</td>
                        <td>Trạng thái</td>
                        <td>Số mặt hàng</td>
                        <td>Người duyệt</td>
                        <td>Thao tác</td>
                    </tr>
                </thead>

                <tbody>
                    {
                        listDestroyReceipt && listDestroyReceipt.length > 0 &&
                        listDestroyReceipt.map((value, index) => {
                            return (
                                <tr key={`table-destroyReceipt-${index}`}>
                                    <td>{value.destroyReceiptID}</td>
                                    <td>{value.stockTakeID}</td>
                                    <td>{value.warehouseName}</td>
                                    <td>{value.userName}</td>
                                    <td>{formatDate(value.createdAt)}</td>
                                    <td>{getStatusBadge(value.status)}</td>
                                    <td>{value.itemCount}</td>
                                    <td>{value.approvedBy ? `${value.approvedBy} (${formatDate(value.approvedAt)})` : '—'}</td>
                                    <td>
                                        <button className="btn btn-secondary btn-sm"
                                            onClick={() => props.handleViewDestroyReceipt(value)}>View</button>
                                        {canApprove(value) &&
                                            <button className="btn btn-success btn-sm mx-1"
                                                onClick={() => props.handleApproveDestroyReceipt(value)}>Duyệt</button>}
                                        {canReject(value) &&
                                            <button className="btn btn-danger btn-sm mx-1"
                                                onClick={() => props.handleRejectDestroyReceipt(value)}>Từ chối</button>}
                                        {canComplete(value) &&
                                            <button className="btn btn-primary btn-sm mx-1"
                                                onClick={() => props.handleCompleteDestroyReceipt(value)}>Hoàn thành</button>}
                                        {canUpdate(value) &&
                                            <button className="btn btn-warning btn-sm mx-1 text-dark"
                                                onClick={() => props.handleUpdateDestroyReceipt(value)}>Sửa phiếu</button>}
                                        {canDelete(value) &&
                                            <button className="btn btn-outline-danger btn-sm mx-1"
                                                onClick={() => props.handleDeleteDestroyReceipt(value)}>Xóa</button>}
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listDestroyReceipt && listDestroyReceipt.length === 0 &&
                        <tr>
                            <td colSpan={9}>"Not found destroy receipt"</td>
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
export default TableDestroyReceipt;
