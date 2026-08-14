import { useState, useEffect } from 'react'
import { getOutOfStock } from '../../../services/apiService'

const OutOfStockList = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getOutOfStock().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot red" />
                Thuốc hết hàng
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Không có thuốc hết hàng</div>}
                {items.map((item, i) => (
                    <div key={i} className="alert-item">
                        <span className="alert-name">{item.medicineName}</span>
                        <span className="alert-qty danger">Cần nhập thêm</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default OutOfStockList
