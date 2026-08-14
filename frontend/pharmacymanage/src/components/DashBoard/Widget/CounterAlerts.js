import { useState, useEffect } from 'react'
import { getCounterAlerts } from '../../../services/apiService'

const CounterAlerts = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getCounterAlerts().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot yellow" />
                Cảnh báo tại quầy
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Đủ hàng</div>}
                {items.map((item, i) => (
                    <div key={i} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-qty danger">Còn {item.quantityInStock} hộp - Báo quản lý nhập thêm</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default CounterAlerts
