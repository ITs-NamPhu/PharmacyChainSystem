import ReactPaginate from "react-paginate";
import { useState, useEffect } from "react";

const TableCustomer = (props) => {
    const { listCustomer, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListCustomer(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    const formatPrice = (price) => {
        if (price == null) return '0';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>CustomerName</td>
                        <td>Phone</td>
                        <td>Address</td>
                        <td>CustomerType</td>
                        <td>Số dư ví</td>
                        <td>Actions</td>
                    </tr>
                </thead>

                <tbody>
                    {listCustomer && listCustomer.length > 0 &&
                        listCustomer.map((value, index) => {
                            return (
                                <tr key={`table-customer-${index}`}>
                                    <td>{value.customerID}</td>
                                    <td>{value.customerName}</td>
                                    <td>{value.phone}</td>
                                    <td>{value.address}</td>
                                    <td>{value.customerTypeName}</td>
                                    <td>
                                        {value.walletBalance > 0
                                            ? <span className="badge bg-success">{formatPrice(value.walletBalance)}đ</span>
                                            : <span className="text-muted">0đ</span>}
                                    </td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateCustomer(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteCustomer(value)}>Delete</button>
                                        <button className="btn btn-info mx-3" onClick={() => props.handleViewWallet(value)}>Xem lịch sử ví</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listCustomer && listCustomer.length === 0 &&
                        <tr>
                            <td colSpan={7}>"Not found customer"</td>
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

export default TableCustomer;
