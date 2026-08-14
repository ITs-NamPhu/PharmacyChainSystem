import { useState, useEffect } from 'react'
import { getShiftRevenue, getShiftOrders } from '../../../services/apiService'

const ShiftKpi = () => {
    const [data, setData] = useState({ revenue: 0, orders: 0 })

    useEffect(() => {
        Promise.all([getShiftRevenue(), getShiftOrders()]).then(([rev, ord]) => {
            setData({
                revenue: rev.data?.DT?.totalRevenue ?? 0,
                orders: ord.data?.DT?.totalOrders ?? 0
            })
        })
    }, [])

    return (
        <div className="kpi-grid">
            <div className="kpi-card">
                <div className="kpi-label">Doanh thu ca của tôi</div>
                <div className="kpi-value">{data.revenue.toLocaleString()}đ</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Hóa đơn đã lập</div>
                <div className="kpi-value">{data.orders}</div>
            </div>
        </div>
    )
}

export default ShiftKpi
