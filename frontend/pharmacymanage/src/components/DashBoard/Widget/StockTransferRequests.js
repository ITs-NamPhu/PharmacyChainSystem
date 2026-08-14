import { useState, useEffect } from 'react'
import { getStockTransferRequests } from '../../../services/apiService'

const StockTransferRequests = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getStockTransferRequests().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">Yêu cầu điều phối (Stock Transfer)</div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Chưa có yêu cầu điều phối</div>}
                {items.map((item, i) => (
                    <div key={i} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-meta">{item.fromBranch} → {item.toBranch}</span>
                        <span className="alert-qty">{item.quantity}</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default StockTransferRequests
