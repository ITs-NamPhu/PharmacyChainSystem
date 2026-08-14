import { useSelector, useDispatch } from 'react-redux';
import { doSetInvoiceBatch } from '../../redux/action/userAction';
import './IndexConfig.scss';

const IndexConfig = () => {
    const dispatch = useDispatch();
    const config = useSelector(state => state.user.config);

    const handleChange = (value) => {
        dispatch(doSetInvoiceBatch(value));
    };

    return (
        <div className="config-container">
            <div className="config-title">Cấu hình</div>
            <div className="config-card">
                <h5>Phương thức xuất kho</h5>
                <div className="config-options">
                    <label className={`config-option ${config === 'lot' ? 'active' : ''}`}>
                        <input
                            type="radio"
                            name="invoiceBatch"
                            value="lot"
                            checked={config === 'lot'}
                            onChange={() => handleChange('lot')}
                        />
                        <div className="option-content">
                            <span className="option-label">Chọn lô</span>
                            <span className="option-desc">Nhân viên tự chọn lô hàng khi xuất kho</span>
                        </div>
                    </label>
                    <label className={`config-option ${config === 'fefo' ? 'active' : ''}`}>
                        <input
                            type="radio"
                            name="invoiceBatch"
                            value="fefo"
                            checked={config === 'fefo'}
                            onChange={() => handleChange('fefo')}
                        />
                        <div className="option-content">
                            <span className="option-label">First Expired First Out (FEFO)</span>
                            <span className="option-desc">Tự động xuất lô hết hạn sớm nhất trước</span>
                        </div>
                    </label>
                </div>
            </div>
        </div>
    );
}

export default IndexConfig;
