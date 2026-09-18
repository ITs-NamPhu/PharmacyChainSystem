import { useState } from 'react';
import './Login.scss'
import { useNavigate } from 'react-router-dom';
import { postLogin } from '../../services/apiService';
import { toast } from 'react-toastify';
import { useDispatch } from 'react-redux';
import { doLogin } from '../../redux/action/userAction';
import { HashLoader } from 'react-spinners';
// import Language from '../Header/Languague';
import { FaEye } from "react-icons/fa";
import { FaEyeSlash } from "react-icons/fa";
import { TbLoaderQuarter } from "react-icons/tb";


const Login = (props) => {
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [isShowPassword, setIsShowPassword] = useState(false);

    const navigate = useNavigate();
    const dispatch = useDispatch();

    const handleLogin = async () => {
        //validate

        setIsLoading(true);
        //submit api
        let res = await postLogin(userName, password);
        if (res && res.ec === 0) {
            dispatch(doLogin(res))
            toast.success(res.em);
            setIsLoading(false);
            navigate('/admin');
        }
        if (res && res.ec !== 0) {
            toast.error(res.em);
            setIsLoading(false);
        }
    }

    const handleKeyDown = (event) => {
        if (event && event.key === 'Enter') {
            handleLogin();
        }
    }
    const handleShowHidePassword = () => {
        setIsShowPassword(!isShowPassword);
    }
    return (
        <div className="login-container">
            <div className='header'>
                <span>Don't have an account yet?</span>
                <button onClick={() => navigate('/signup')}>Sign up</button>
                {/* <Language /> */}
            </div>
            <div className='title col-4 mx-auto'>
                Login
            </div>

            <div className='welcome col-4 mx-auto'>
                Hello, who's this?
            </div>

            <div className='content-form col-4 mx-auto'>
                <div className='form-group'>
                    <label>UserName</label>
                    <input type={"text"} value={userName} className='form-control' onChange={(event) => setUserName(event.target.value)} />
                </div>
                <div className='form-group div-password'>
                    <label>Password</label>
                    <input
                        type={isShowPassword === true ? "text" : "password"}
                        value={password}
                        className='form-control'
                        onChange={(event) => setPassword(event.target.value)}
                        onKeyDown={(event) => handleKeyDown(event)}
                    />
                    <span className='forgot-password'>Forgot password?</span>
                    <div>
                        <button
                            className='btn-submit'
                            disabled={isLoading}
                            onClick={() => handleLogin()}
                        >
                            {isLoading === true && <TbLoaderQuarter size={20} />}
                            Login
                        </button>
                    </div>
                    {/* <div className='text-center'>
                        <span className='back' onClick={() => navigate('/')}> &#60;&#60;Go back homepage</span>
                    </div> */}

                    {isShowPassword === false ?
                        <span className='showEye' onClick={() => handleShowHidePassword()}> <FaEye /> </span>
                        :
                        <span className='showEye' onClick={() => handleShowHidePassword()
                        }> <FaEyeSlash /> </span>
                    }
                </div>
            </div>

        </div>
    );
}
export default Login;