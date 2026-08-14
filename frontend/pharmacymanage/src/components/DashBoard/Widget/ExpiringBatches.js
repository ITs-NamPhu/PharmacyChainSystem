import { useState, useEffect } from 'react'
import { getExpiringBatches } from '../../../services/apiService'

const ExpiringBatches = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getExpiringBatches().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot yellow" />
                Cảnh báo Hạn sử dụng
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Không có cảnh báo</div>}
                {items.map(item => (
                    <div key={item.batchID} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-meta">HSD: {new Date(item.expiryDate).toLocaleDateString('vi-VN')} ({item.daysUntilExpiry} ngày)</span>
                        <span className="alert-qty">{item.quantityInStock} hộp</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default ExpiringBatches
