import { useState, useEffect } from 'react'
import { getLowStock } from '../../../services/apiService'

const LowStockAlerts = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getLowStock(10).then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot red" />
                Cảnh báo Tồn kho thấp
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Đủ hàng</div>}
                {items.map((item, i) => (
                    <div key={i} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-meta">{item.branchName}</span>
                        <span className="alert-qty danger">{item.quantityInStock} hộp</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default LowStockAlerts
