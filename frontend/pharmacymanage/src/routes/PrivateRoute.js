import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

const PrivateRoute = (props) => {
    const isAuthentication = useSelector(state => state.user.isAuthentication);

    if (!isAuthentication) {
        return <Navigate to="/" > </Navigate>
    }
    return (
        <>
            {props.children}
        </>
    )
}
export default PrivateRoute