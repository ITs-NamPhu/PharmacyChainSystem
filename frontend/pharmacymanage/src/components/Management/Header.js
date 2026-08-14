import './Header.scss';
import { useSelector, useDispatch } from 'react-redux';
import { doSetBranchId } from '../../redux/action/userAction';

const Header = (props) => {
    const { onToggle, time } = props;
    const dispatch = useDispatch();

    const Information_user = useSelector(state => state.user.account)
    const roleName = Information_user.roleName;
    const branches = Information_user.branches || [];
    const currentBranchId = Information_user.currentBranchId;

    const canChangeBranch = roleName === 'admin' || roleName === 'manage_supply';

    const handleBranchChange = (e) => {
        const newBranchId = Number(e.target.value);
        dispatch(doSetBranchId(newBranchId));
    };
    // console.log(branches)
    return (
        <div className="content-header">
            <button className="sidebar-toggle" onClick={onToggle} title="Thu gọn / Mở rộng sidebar" aria-label="Toggle sidebar">
                <svg viewBox="0 0 24 24" fill="none" width="20" height="20">
                    <path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                </svg>
            </button>

            <div className="header-title">
                <h1>Chào buổi sáng, {Information_user.fullName} 👋</h1>
                <p>{time}</p>
            </div>

            <div className="header-search">
                <svg viewBox="0 0 24 24" fill="none" width="18" height="18"><circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="1.8" /><path d="M21 21l-4-4" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" /></svg>
                <input type="text" placeholder="Tìm thuốc, đơn hàng, khách hàng…" />
                <kbd>Ctrl K</kbd>
            </div>

            <div className="header-actions">
                {branches.length > 0 && (
                    <select
                        className="form-select form-select-sm branch-select"
                        value={currentBranchId}
                        onChange={handleBranchChange}
                        disabled={!canChangeBranch}
                    >
                        {branches.map((branch) => (
                            <option key={branch.branchId} value={branch.branchId}>
                                {branch.branchName}
                            </option>
                        ))}
                    </select>
                )}
                <button className="icon-btn" title="Thông báo">
                    <svg viewBox="0 0 24 24" fill="none" width="20" height="20"><path d="M18 8a6 6 0 00-12 0c0 7-3 9-3 9h18s-3-2-3-9" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" /><path d="M13.7 21a2 2 0 01-3.4 0" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" /></svg>
                    <span className="dot"></span>
                </button>
            </div>
        </div>
    );
}

export default Header;
