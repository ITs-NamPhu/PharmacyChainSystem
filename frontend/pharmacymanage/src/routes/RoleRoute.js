import { Navigate } from "react-router-dom";
import { useSelector } from "react-redux";

const RoleRoute = ({ roles, children }) => {

    const role = useSelector(
        state => state.user.account.roleName
    );
    console.log("RoleRoute role: ", role);
    if (!roles.includes(role))
        return <Navigate to="/403" replace />

    return children;
}

export default RoleRoute;