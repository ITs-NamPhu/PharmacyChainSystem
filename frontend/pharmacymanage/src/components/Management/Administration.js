import { useState, useEffect } from 'react';
import './Administration.scss';
import Sidebar from './Sidebar';
import Header from './Header';
import Footer from './Footer';
import ChatWidget from '../ChatWidget/ChatWidget';
import { Outlet } from "react-router-dom";

const Administration = (props) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const toggleSidebar = () => setIsCollapsed(prev => !prev);

    const [now, setNow] = useState(new Date());

    useEffect(() => {
        const timer = setInterval(() => {
            setNow(new Date());
        }, 5000);

        return () => clearInterval(timer);
    }, []);


    const weekday = now.toLocaleDateString("vi-VN", {
        weekday: "long",
    });
    const date = now.toLocaleDateString("vi-VN");
    const time = now.toLocaleTimeString("vi-VN");
    return (
        <div className={`container${isCollapsed ? ' sidebar-collapsed' : ''}`}>
            <div className={`sidebar${isCollapsed ? ' is-collapsed' : ''}`}>
                <Sidebar isCollapsed={isCollapsed} onToggle={toggleSidebar} />
            </div>
            <div className='content'>
                <Header onToggle={toggleSidebar} time={`${weekday}, ${date} • ${time}`} />
                <div className='content-middle'>
                    <Outlet />
                </div>
                <Footer />
            </div>
            <ChatWidget />
        </div>
    );
}

export default Administration;