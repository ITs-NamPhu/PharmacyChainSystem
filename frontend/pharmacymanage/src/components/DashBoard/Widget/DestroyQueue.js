import { useState, useEffect } from 'react'
import { getDestroyQueue } from '../../../services/apiService'

const DestroyQueue = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getDestroyQueue().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot red" />
                Thuốc chờ tiêu hủy
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Không có</div>}
                {items.map(item => (
                    <div key={item.batchID} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-meta">{item.branchName} - HSD: {new Date(item.expiryDate).toLocaleDateString('vi-VN')}</span>
                        <span className="alert-qty danger">{item.quantityInStock} hộp</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default DestroyQueue
