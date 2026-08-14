import './Footer.scss';
const Footer = (props) => {
    const { time } = props;
    return (
        <div className="content-footer">
            <div className="footer-inner">
                <div className="bb-item">
                    <span className="bb-dot online"></span>
                    Hệ thống hoạt động bình thường
                </div>
                <div className="bb-sep"></div>
                <div className="bb-item mono">Ca hiện tại: 00:00 – 24:00</div>
                <div className="bb-sep"></div>
                <div className="bb-item mono">Máy in: Sẵn sàng</div>
                <div className="bb-spacer"></div>
                <div className="bb-item mono">NHATHUOC OS v1.4.0</div>
            </div>
        </div>
    );
}

export default Footer;