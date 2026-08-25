import { useState } from 'react';
import { Collapse } from 'react-bootstrap';
import { AiOutlineDown, AiOutlineUp } from 'react-icons/ai';

const FilterPanel = ({ filters, onFilterChange, onReset, children }) => {
    const [open, setOpen] = useState(false);

    const handleSelectChange = (key, value) => {
        onFilterChange(key, value);
    };

    return (
        <div className="filter-panel mb-3">
            <button
                className="btn btn-outline-secondary btn-sm d-flex align-items-center gap-1"
                onClick={() => setOpen(!open)}
                type="button"
            >
                Bộ lọc nâng cao {open ? <AiOutlineUp /> : <AiOutlineDown />}
            </button>

            <Collapse in={open}>
                <div className="filter-panel-content mt-2 p-3 border rounded bg-light">
                    <div className="row g-2">
                        {children}
                    </div>
                    <div className="mt-2 d-flex gap-2">
                        <button className="btn btn-primary btn-sm" onClick={() => onFilterChange('__apply')}>
                            Áp dụng
                        </button>
                        <button className="btn btn-outline-danger btn-sm" onClick={onReset}>
                            Xóa lọc
                        </button>
                    </div>
                </div>
            </Collapse>
        </div>
    );
};

export const FilterSelect = ({ label, value, options, onChange }) => (
    <div className="col-md-3 col-sm-6">
        <label className="form-label fw-semibold" style={{ fontSize: '0.85rem' }}>{label}</label>
        <select
            className="form-select form-select-sm"
            value={value}
            onChange={(e) => onChange(e.target.value)}
        >
            {options.map((opt) => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
        </select>
    </div>
);

export const FilterDateRange = ({ labelFrom, valueFrom, labelTo, valueTo, onChangeFrom, onChangeTo }) => (
    <>
        <div className="col-md-3 col-sm-6">
            <label className="form-label fw-semibold" style={{ fontSize: '0.85rem' }}>{labelFrom}</label>
            <input
                type="date"
                className="form-control form-control-sm"
                value={valueFrom || ''}
                onChange={(e) => onChangeFrom(e.target.value || null)}
            />
        </div>
        <div className="col-md-3 col-sm-6">
            <label className="form-label fw-semibold" style={{ fontSize: '0.85rem' }}>{labelTo}</label>
            <input
                type="date"
                className="form-control form-control-sm"
                value={valueTo || ''}
                onChange={(e) => onChangeTo(e.target.value || null)}
            />
        </div>
    </>
);

export const FilterNumber = ({ label, value, min, max, onChange }) => (
    <div className="col-md-3 col-sm-6">
        <label className="form-label fw-semibold" style={{ fontSize: '0.85rem' }}>{label}</label>
        <input
            type="number"
            className="form-control form-control-sm"
            value={value ?? ''}
            min={min}
            max={max}
            onChange={(e) => onChange(e.target.value ? Number(e.target.value) : null)}
        />
    </div>
);

export default FilterPanel;
