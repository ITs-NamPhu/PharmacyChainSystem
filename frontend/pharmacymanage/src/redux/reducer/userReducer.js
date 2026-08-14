import { Fetch_user_login_success, User_logout_success, User_refreshToken_success, Set_BranchId, Set_InvoiceBatch } from "../action/userAction";

const init_state = {
    account: {
        access_token: '',
        refresh_token: '',
        userId: '',
        userName: '',
        fullName: '',
        roleID: '',
        roleName: '',
        branches: [],
        currentBranchId: ''
    },
    isAuthentication: false,
    config: 'fefo'
}

const userReducer = (state = init_state, action) => {
    switch (action.type) {
        case Fetch_user_login_success: {
            const dt = action?.payload?.dt;
            const user = dt?.user || {};
            const branches = dt?.branches || [];
            const defaultBranch = branches.find(x => x.isDefault);
            return {
                ...state,
                account: {
                    access_token: dt?.accessToken || '',
                    refresh_token: dt?.refreshToken || '',
                    userId: user.userId || '',
                    userName: user.userName || '',
                    fullName: user.fullName || '',
                    roleID: user.roleID || '',
                    roleName: user.role || '',
                    branches: branches,
                    currentBranchId: defaultBranch?.branchId || ''
                },
                isAuthentication: true
            };
        }
        case User_logout_success:
            return {
                ...state,
                account: {
                    access_token: '',
                    refresh_token: '',
                    userId: '',
                    userName: '',
                    fullName: '',
                    roleID: '',
                    roleName: '',
                    branches: [],
                    currentBranchId: ''
                },
                isAuthentication: false
            };
        case User_refreshToken_success: {
            const dt = action?.payload?.dt;
            const user = dt?.user || {};
            const branches = dt?.branches || [];
            const defaultBranch = branches.find(x => x.isDefault);
            return {
                ...state,
                account: {
                    ...state.account,
                    access_token: dt?.accessToken || '',
                    refresh_token: dt?.refreshToken || '',
                    userId: user.userId || '',
                    userName: user.userName || '',
                    fullName: user.fullName || '',
                    roleID: user.roleID || '',
                    roleName: user.role || '',
                    branches: branches,
                    currentBranchId: defaultBranch?.branchId || state.account.currentBranchId
                }
            };
        }
        case Set_BranchId:
            return {
                ...state,
                account: {
                    ...state.account,
                    currentBranchId: action.payload
                }
            };
        case Set_InvoiceBatch:
            return {
                ...state,
                config: action.payload
            };
        default:
            return state;
    }
}
export default userReducer
