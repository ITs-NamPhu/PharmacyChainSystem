import ReactPaginate from "react-paginate";

const TableStockTake = (props) => {
    const { listStockTake, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchStockTake(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    const formatDate = (dateStr) => {
        if (!dateStr) return "";
        const d = new Date(dateStr);
        return d.toLocaleDateString("vi-VN") + " " + d.toLocaleTimeString("vi-VN");
    }

    const getStatusBadge = (status) => {
        switch (status) {
            case "Draft":
                return <span className="badge bg-warning text-dark">Nháp</span>;
            case "Completed":
                return <span className="badge bg-success">Hoàn thành</span>;
            case "Cancelled":
                return <span className="badge bg-danger">Đã hủy</span>;
            default:
                return <span className="badge bg-secondary">{status}</span>;
        }
    }

    const getBalanceBadge = (isBalance) => {
        if (isBalance === "Balanced")
            return <span className="badge bg-success">Khớp</span>;
        return <span className="badge bg-danger">Lệch</span>;
    }

    const canComplete = (value) => value.status === "Draft";
    const canCancel = (value) => value.status === "Draft";
    const canApprove = (value) => value.status === "Completed" && !value.approvedBy;

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Kho</td>
                        <td>Người tạo</td>
                        <td>Ngày tạo</td>
                        <td>Trạng thái</td>
                        <td>Kết quả</td>
                        <td>Số mặt hàng</td>
                        <td>Đã điều chỉnh</td>
                        <td>Người duyệt</td>
                        <td>Thao tác</td>
                    </tr>
                </thead>

                <tbody>
                    {
                        listStockTake && listStockTake.length > 0 &&
                        listStockTake.map((value, index) => {
                            return (
                                <tr key={`table-stockTake-${index}`}>
                                    <td>{value.stockTakeID}</td>
                                    <td>{value.warehouseName}</td>
                                    <td>{value.userName}</td>
                                    <td>{formatDate(value.createdAt)}</td>
                                    <td>{getStatusBadge(value.status)}</td>
                                    <td>{getBalanceBadge(value.isBalance)}</td>
                                    <td>{value.itemCount}</td>
                                    <td>{value.isAdjusted ? 'Có' : 'Không'}</td>
                                    <td>{value.approvedBy ? `${value.approvedBy} (${formatDate(value.approvedAt)})` : '—'}</td>
                                    <td>
                                        <button className="btn btn-secondary btn-sm"
                                            onClick={() => props.handleViewStockTake(value)}>View</button>
                                        {canComplete(value) &&
                                            <button className="btn btn-success btn-sm mx-1"
                                                onClick={() => props.handleCompleteStockTake(value)}>Complete</button>}
                                        {canCancel(value) &&
                                            <button className="btn btn-danger btn-sm"
                                                onClick={() => props.handleCancelStockTake(value)}>Cancel</button>}
                                        {canApprove(value) &&
                                            <button className="btn btn-primary btn-sm mx-1"
                                                onClick={() => props.handleApproveStockTake(value)}>Approve</button>}
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listStockTake && listStockTake.length === 0 &&
                        <tr>
                            <td colSpan={10}>"Not found stock take"</td>
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
export default TableStockTake;
