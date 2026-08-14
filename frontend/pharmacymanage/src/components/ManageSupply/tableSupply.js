import ReactPaginate from "react-paginate";

const TableSupply = (props) => {
    const { listSupply, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListSupply(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>SupplierName</td>
                        <td>Phone</td>
                        <td>Email</td>
                        <td>Address</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listSupply && listSupply.length > 0 &&
                        listSupply.map((value, index) => {
                            return (
                                <tr key={`table-supply-${index}`}>
                                    <td>{value.supplierID}</td>
                                    <td>{value.supplierName}</td>
                                    <td>{value.phone}</td>
                                    <td>{value.email}</td>
                                    <td>{value.address}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateSupply(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteSupply(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listSupply && listSupply.length === 0 &&
                        <tr>
                            <td colSpan={6}>"Not found supplier"</td>
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
export default TableSupply;
