import ReactPaginate from "react-paginate";

const TableUnit = (props) => {
    const { listUnit, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListUnit(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1)
    }

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>UnitName</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listUnit && listUnit.length > 0 &&
                        listUnit.map((value, index) => {
                            return (
                                <tr key={`table-unit-${index}`}>
                                    <td>{value.unitID}</td>
                                    <td>{value.unitName}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateUnit(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteUnit(value)}>Delete</button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                    {listUnit && listUnit.length === 0 &&
                        <tr>
                            <td colSpan={3}>"Not found unit"</td>
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
export default TableUnit;
