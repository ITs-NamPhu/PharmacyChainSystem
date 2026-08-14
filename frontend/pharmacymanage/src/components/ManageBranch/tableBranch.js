import ReactPaginate from "react-paginate";
import { useState, useEffect } from "react";

const TableBranch = (props) => {
    const { listBranch, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListBranch(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>BranchName</td>
                        <td>Phone</td>
                        <td>Address</td>
                        <td>Actions</td>
                    </tr>
                </thead>

                <tbody>
                    {listBranch && listBranch.length > 0 &&
                        listBranch.map((value, index) => {
                            return (
                                <tr key={`table-branch-${index}`}>
                                    <td>{value.branchID}</td>
                                    <td>{value.branchName}</td>
                                    <td>{value.phone}</td>
                                    <td>{value.address}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateBranch(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteBranch(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listBranch && listBranch.length === 0 &&
                        <tr>
                            <td colSpan={4}>"Not found branch"</td>
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

export default TableBranch;