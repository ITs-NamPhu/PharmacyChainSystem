import ReactPaginate from "react-paginate";

const TableManufacturer = (props) => {
    const { listManufacturer, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListManufacturer(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>ManufacturerName</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listManufacturer && listManufacturer.length > 0 &&
                        listManufacturer.map((value, index) => {
                            return (
                                <tr key={`table-manufacturer-${index}`}>
                                    <td>{value.manufacturerID}</td>
                                    <td>{value.manufacturerName}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateManufacturer(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteManufacturer(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listManufacturer && listManufacturer.length === 0 &&
                        <tr>
                            <td colSpan={3}>"Not found manufacturer"</td>
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
export default TableManufacturer;
