export const Fetch_user_login_success = 'Fetch_user_login_success'
export const User_logout_success = 'User_logout_success'
export const User_refreshToken_success = 'User_refreshToken_success'
export const Set_BranchId = 'Set_BranchId'
export const Set_InvoiceBatch = 'Set_InvoiceBatch'

export const doLogin = (res) => {
    return { type: Fetch_user_login_success, payload: res };
}

export const doLogout = () => {
    return { type: User_logout_success };
}

export const doRefreshToken = (res) => {
    return { type: User_refreshToken_success, payload: res };
}

export const doSetBranchId = (branchId) => {
    return { type: Set_BranchId, payload: branchId };
}

export const doSetInvoiceBatch = (value) => {
    return { type: Set_InvoiceBatch, payload: value };
}
