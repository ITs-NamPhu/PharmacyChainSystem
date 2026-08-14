import ReactPaginate from "react-paginate";
import { useState, useEffect } from "react";

const TableUser = (props) => {
    const { listUser, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListUserPag(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>FullName</td>
                        <td>Address</td>
                        <td>Email</td>
                        <td>Role</td>
                        <td>Action</td>
                    </tr>
                </thead>

                <tbody>
                    {
                        listUser && listUser.length > 0 &&
                        listUser.map((value, index) => {
                            return (
                                <tr key={`table-user-${index}`}>
                                    <td>{value.userID}</td>
                                    <td>{value.fullName}</td>
                                    <td>{value.address}</td>
                                    <td>{value.email}</td>
                                    <td>{value.roleName}</td>

                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateUser(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteUser(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listUser && listUser.length === 0 &&
                        <tr>
                            <td colSpan={4}>"Not found user"</td>
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

export default TableUser;