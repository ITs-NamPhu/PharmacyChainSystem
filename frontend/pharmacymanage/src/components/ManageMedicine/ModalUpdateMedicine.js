import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Tabs from 'react-bootstrap/Tabs';
import Tab from 'react-bootstrap/Tab';
import { FaPlus, FaTrash } from 'react-icons/fa';
import { toast } from 'react-toastify';
import _ from 'lodash';
import {
    UpdateMedicine,
    GetMedicineById,
    CreateUnitConversion,
    DeleteUnitConversion,
    GetAllUnitConversion,
    GetAllUnitNoPag,
    getAllManufacturerAll,
    getAllMedicineCategoryAll
} from '../../services/apiService';

const ModalUpdateMedicine = (props) => {
    const { show, setShow, dataUpdate } = props;

    const [activeTab, setActiveTab] = useState('medicineInfo');

    const [medicineName, setMedicineName] = useState('');
    const [defaultRetailPrice, setDefaultRetailPrice] = useState('');
    const [defaultWholesalePrice, setDefaultWholesalePrice] = useState('');
    const [vatPercent, setVatPercent] = useState('');
    const [categoryID, setCategoryID] = useState('');
    const [manufacturerID, setManufacturerID] = useState('');
    const [baseUnitID, setBaseUnitID] = useState('');

    const [listUnit, setListUnit] = useState([]);
    const [listManufacturer, setListManufacturer] = useState([]);
    const [listMedicineCategory, setListMedicineCategory] = useState([]);

    const [unitConversions, setUnitConversions] = useState([]);
    const [existingConversionIDs, setExistingConversionIDs] = useState([]);

    useEffect(() => {
        if (show && !_.isEmpty(dataUpdate)) {
            fetchMedicineDetail();
            fetchDropdownData();
        }
    }, [show, dataUpdate]);

    const fetchMedicineDetail = async () => {
        let res = await GetMedicineById(dataUpdate.medicineID);
        if (res && res.ec === 0) {
            const m = res.dt;
            setMedicineName(m.medicineName || '');
            setDefaultRetailPrice(m.defaultRetailPrice || '');
            setDefaultWholesalePrice(m.defaultWholesalePrice || '');
            setVatPercent(m.vatPercent || '');
            setCategoryID(m.categoryID || '');
            setManufacturerID(m.manufacturerID || '');
            setBaseUnitID(m.baseUnitID || '');
        }
    };

    const fetchUnitConversions = async () => {
        let res = await GetAllUnitConversion();
        if (res && res.ec === 0) {
            const filtered = res.dt.filter(uc => uc.medicineID === dataUpdate.medicineID);
            setUnitConversions(filtered.map(uc => ({
                unitConversionID: uc.unitConversionID,
                unitID: uc.unitID,
                factor: uc.factor
            })));
            setExistingConversionIDs(filtered.map(uc => uc.unitConversionID));
        }
    };

    const fetchDropdownData = async () => {
        let [resUnit, resManu, resCat] = await Promise.all([
            GetAllUnitNoPag(),
            getAllManufacturerAll(),
            getAllMedicineCategoryAll()
        ]);
        if (resUnit && resUnit.ec === 0) setListUnit(resUnit.dt);
        if (resManu && resManu.ec === 0) setListManufacturer(resManu.dt);
        if (resCat && resCat.ec === 0) setListMedicineCategory(resCat.dt);

        await fetchUnitConversions();
    };

    const handleClose = () => {
        setShow(false);
        setActiveTab('medicineInfo');
        setMedicineName('');
        setDefaultRetailPrice('');
        setDefaultWholesalePrice('');
        setVatPercent('');
        setCategoryID('');
        setManufacturerID('');
        setBaseUnitID('');
        setUnitConversions([]);
        setExistingConversionIDs([]);
    };

    const getBaseUnitName = () => {
        const found = listUnit.find(u => u.unitID === +baseUnitID);
        return found ? found.unitName : '';
    };

    const getUnitName = (unitID) => {
        const found = listUnit.find(u => u.unitID === +unitID);
        return found ? found.unitName : '';
    };

    const getAvailableUnits = (excludeUnitID) => {
        return listUnit.filter(u => u.unitID !== +baseUnitID && u.unitID !== +excludeUnitID);
    };

    const handleAddUnitConversion = () => {
        setUnitConversions([...unitConversions, { unitID: '', factor: '' }]);
    };

    const handleRemoveUnitConversion = (index) => {
        const updated = [...unitConversions];
        updated.splice(index, 1);
        setUnitConversions(updated);
    };

    const handleChangeUnitConversion = (index, field, value) => {
        const updated = [...unitConversions];
        updated[index][field] = value;
        setUnitConversions(updated);
    };

    const handleSubmitMedicine = async () => {
        if (!medicineName || medicineName.trim() === '') {
            toast.error('Vui lòng nhập tên thuốc');
            setActiveTab('medicineInfo');
            return;
        }
        if (!categoryID || categoryID === '') {
            toast.error('Vui lòng chọn danh mục');
            setActiveTab('medicineInfo');
            return;
        }
        if (!manufacturerID || manufacturerID === '') {
            toast.error('Vui lòng chọn nhà sản xuất');
            setActiveTab('medicineInfo');
            return;
        }
        if (!defaultRetailPrice || +defaultRetailPrice <= 0) {
            toast.error('Giá bán lẻ phải lớn hơn 0');
            setActiveTab('medicineInfo');
            return;
        }
        if (!defaultWholesalePrice || +defaultWholesalePrice <= 0) {
            toast.error('Giá bán sỉ phải lớn hơn 0');
            setActiveTab('medicineInfo');
            return;
        }
        if (!baseUnitID || baseUnitID === '') {
            toast.error('Vui lòng chọn đơn vị cơ bản');
            setActiveTab('unitConversion');
            return;
        }

        let res = await UpdateMedicine(
            dataUpdate.medicineID,
            medicineName.trim(),
            +defaultRetailPrice,
            +defaultWholesalePrice,
            +vatPercent || 0,
            +categoryID,
            +manufacturerID,
            +baseUnitID
        );

        if (!res || res.ec !== 0) {
            toast.error(res?.EM || 'Cập nhật thuốc thất bại');
            return;
        }

        const currentConversionIDs = unitConversions
            .filter(uc => uc.unitConversionID)
            .map(uc => uc.unitConversionID);

        for (let oldID of existingConversionIDs) {
            if (!currentConversionIDs.includes(oldID)) {
                await DeleteUnitConversion(oldID);
            }
        }

        for (let uc of unitConversions) {
            if (!uc.unitConversionID && uc.unitID && uc.factor && +uc.factor > 0) {
                await CreateUnitConversion(+uc.unitID, dataUpdate.medicineID, +uc.factor);
            }
        }

        toast.success(res.EM || 'Cập nhật thuốc thành công');
        handleClose();
        await props.fetchListMedicine(1);
    };

    return (
        <>
            <Modal show={show} onHide={handleClose} size="xl" className='modal-update-medicine'>
                <Modal.Header closeButton>
                    <Modal.Title>Cập nhật thuốc</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Tabs
                        activeKey={activeTab}
                        onSelect={(k) => setActiveTab(k)}
                        className="mb-3"
                    >
                        <Tab eventKey="medicineInfo" title="Thông tin thuốc">
                            <form className='row g-3'>
                                <div className="form-group col-md-6">
                                    <label>Tên thuốc <span className="text-danger">*</span></label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={medicineName}
                                        placeholder="Nhập tên thuốc"
                                        onChange={(event) => setMedicineName(event.target.value)}
                                    />
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Danh mục <span className="text-danger">*</span></label>
                                    <select className="form-control" value={categoryID} onChange={(event) => setCategoryID(event.target.value)}>
                                        <option value="">-- Chọn danh mục --</option>
                                        {listMedicineCategory && listMedicineCategory.length > 0 &&
                                            listMedicineCategory.map((item, index) => (
                                                <option key={`option-cat-${index}`} value={item.medicineCategoryID}>{item.categoryName}</option>
                                            ))
                                        }
                                    </select>
                                </div>
                                <div className="form-group col-md-6">
                                    <label>Nhà sản xuất <span className="text-danger">*</span></label>
                                    <select className="form-control" value={manufacturerID} onChange={(event) => setManufacturerID(event.target.value)}>
                                        <option value="">-- Chọn nhà sản xuất --</option>
                                        {listManufacturer && listManufacturer.length > 0 &&
                                            listManufacturer.map((item, index) => (
                                                <option key={`option-manu-${index}`} value={item.manufacturerID}>{item.manufacturerName}</option>
                                            ))
                                        }
                                    </select>
                                </div>
                                <div className="form-group col-md-4">
                                    <label>Giá bán lẻ (đ) <span className="text-danger">*</span></label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={defaultRetailPrice}
                                        placeholder="0"
                                        min="0"
                                        onChange={(event) => setDefaultRetailPrice(event.target.value)}
                                    />
                                </div>
                                <div className="form-group col-md-4">
                                    <label>Giá bán sỉ (đ) <span className="text-danger">*</span></label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={defaultWholesalePrice}
                                        placeholder="0"
                                        min="0"
                                        onChange={(event) => setDefaultWholesalePrice(event.target.value)}
                                    />
                                </div>
                                <div className="form-group col-md-4">
                                    <label>VAT (%)</label>
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={vatPercent}
                                        placeholder="0"
                                        min="0"
                                        max="100"
                                        onChange={(event) => setVatPercent(event.target.value)}
                                    />
                                </div>
                            </form>
                        </Tab>

                        <Tab eventKey="unitConversion" title="Đơn vị tính & Quy đổi">
                            <div className='unit-conversion-tab'>
                                <div className='form-group mb-3'>
                                    <label className='fw-bold'>
                                        Đơn vị cơ bản (nhỏ nhất để chia liều/bán lẻ)
                                    </label>
                                    <select
                                        className="form-control"
                                        value={baseUnitID}
                                        onChange={(event) => {
                                            setBaseUnitID(event.target.value);
                                            setUnitConversions([]);
                                        }}
                                    >
                                        <option value="">-- Chọn đơn vị cơ bản --</option>
                                        {listUnit && listUnit.length > 0 &&
                                            listUnit.map((item, index) => (
                                                <option key={`option-baseunit-${index}`} value={item.unitID}>{item.unitName}</option>
                                            ))
                                        }
                                    </select>
                                </div>

                                {baseUnitID && (
                                    <>
                                        <div className='d-flex justify-content-between align-items-center mb-2'>
                                            <label className='fw-bold mb-0'>Bảng quy đổi đơn vị</label>
                                            <button
                                                type="button"
                                                className='btn btn-success btn-sm'
                                                onClick={handleAddUnitConversion}
                                            >
                                                <FaPlus /> Thêm đơn vị
                                            </button>
                                        </div>

                                        <table className="table table-bordered table-sm">
                                            <thead className="table-light">
                                                <tr>
                                                    <th style={{ width: '25%' }}>Đơn vị quy đổi</th>
                                                    <th style={{ width: '25%' }}>Bao nhiêu {getBaseUnitName()}?</th>
                                                    <th style={{ width: '35%' }}>Mô tả</th>
                                                    <th style={{ width: '15%' }}>Thao tác</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {unitConversions.length === 0 &&
                                                    <tr>
                                                        <td colSpan={4} className="text-center text-muted">
                                                            Chưa có đơn vị quy đổi. Nhấn "+ Thêm đơn vị" để thêm.
                                                        </td>
                                                    </tr>
                                                }
                                                {unitConversions.map((uc, index) => {
                                                    const selectedUnitName = getUnitName(uc.unitID);
                                                    const baseUnitName = getBaseUnitName();
                                                    const desc = uc.unitID && uc.factor
                                                        ? `1 ${selectedUnitName} = ${uc.factor} ${baseUnitName}`
                                                        : '';
                                                    return (
                                                        <tr key={`uc-row-${index}`}>
                                                            <td>
                                                                <select
                                                                    className="form-control form-control-sm"
                                                                    value={uc.unitID}
                                                                    onChange={(event) =>
                                                                        handleChangeUnitConversion(index, 'unitID', event.target.value)
                                                                    }
                                                                >
                                                                    <option value="">-- Chọn đơn vị --</option>
                                                                    {getAvailableUnits(uc.unitID).map((item, idx) => (
                                                                        <option key={`uc-unit-${index}-${idx}`} value={item.unitID}>
                                                                            {item.unitName}
                                                                        </option>
                                                                    ))}
                                                                </select>
                                                            </td>
                                                            <td>
                                                                <input
                                                                    type="number"
                                                                    className="form-control form-control-sm"
                                                                    value={uc.factor}
                                                                    min="1"
                                                                    placeholder="1"
                                                                    onChange={(event) =>
                                                                        handleChangeUnitConversion(index, 'factor', event.target.value)
                                                                    }
                                                                />
                                                            </td>
                                                            <td>
                                                                <span className="text-muted">{desc}</span>
                                                            </td>
                                                            <td className="text-center">
                                                                <button
                                                                    type="button"
                                                                    className="btn btn-danger btn-sm"
                                                                    onClick={() => handleRemoveUnitConversion(index)}
                                                                >
                                                                    <FaTrash />
                                                                </button>
                                                            </td>
                                                        </tr>
                                                    );
                                                })}
                                            </tbody>
                                        </table>
                                    </>
                                )}

                                {!baseUnitID && (
                                    <div className="text-center text-muted mt-3">
                                        Vui lòng chọn đơn vị cơ bản để thiết lập quy đổi.
                                    </div>
                                )}
                            </div>
                        </Tab>
                    </Tabs>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                    <Button variant="primary" onClick={() => handleSubmitMedicine()}>
                        Lưu thay đổi
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default ModalUpdateMedicine;
