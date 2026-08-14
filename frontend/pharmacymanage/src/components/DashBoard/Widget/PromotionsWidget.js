import { useState, useEffect } from 'react'
import { getPromotions } from '../../../services/apiService'

const PromotionsWidget = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getPromotions().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">
                <span className="alert-dot green" />
                Chương trình khuyến mãi hôm nay
            </div>
            <div className="widget-list">
                {items.length === 0 && <div className="empty-state">Hôm nay không có khuyến mãi</div>}
                {items.map(item => (
                    <div key={item.promotionID} className="promo-item">
                        <div className="promo-name">{item.promotionName}</div>
                        {item.endDate && <div className="promo-meta">Đến {new Date(item.endDate).toLocaleDateString('vi-VN')}</div>}
                    </div>
                ))}
            </div>
        </div>
    )
}

export default PromotionsWidget
