import ReactPaginate from "react-paginate";

const TableMedicine = (props) => {
    const { listMedicine, pageCount } = props;

    const handlePageClick = (event) => {
        props.fetchListMedicine(+event.selected + 1);
        props.setCurrentPage(+event.selected + 1);
    };

    const formatPrice = (price) => {
        if (price == null) return '';
        return new Intl.NumberFormat('vi-VN').format(price);
    };

    return (
        <>
            <table className="table table-hover table-bordered">
                <thead>
                    <tr scope="col">
                        <td>ID</td>
                        <td>Tên thuốc</td>
                        <td>Giá bán lẻ</td>
                        <td>Giá bán sỉ</td>
                        <td>VAT %</td>
                        <td>Danh mục</td>
                        <td>Nhà sản xuất</td>
                        <td>Đơn vị</td>
                        <td>Actions</td>
                    </tr>
                </thead>
                <tbody>
                    {listMedicine && listMedicine.length > 0 &&
                        listMedicine.map((value, index) => {
                            return (
                                <tr key={`table-medicine-${index}`}>
                                    <td>{value.medicineID}</td>
                                    <td>{value.medicineName}</td>
                                    <td>{formatPrice(value.defaultRetailPrice)}</td>
                                    <td>{formatPrice(value.defaultWholesalePrice)}</td>
                                    <td>{value.vatPercent}</td>
                                    <td>{value.categoryName}</td>
                                    <td>{value.manufacturerName}</td>
                                    <td>{value.unitName}</td>
                                    <td>
                                        <button className="btn btn-secondary">View</button>
                                        <button className="btn btn-warning mx-3" onClick={() => props.handleUpdateMedicine(value)}>Update</button>
                                        <button className="btn btn-danger" onClick={() => props.handleDeleteMedicine(value)}>Delete</button>
                                    </td>
                                </tr>
                            );
                        })
                    }
                    {listMedicine && listMedicine.length === 0 &&
                        <tr>
                            <td colSpan={9}>"Not found medicine"</td>
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
};

export default TableMedicine;
