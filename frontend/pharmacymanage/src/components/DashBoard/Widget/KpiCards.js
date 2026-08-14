import { useState, useEffect } from 'react'
import { getTotalRevenue, getTotalOrders, getNewCustomers } from '../../../services/apiService'

const KpiCards = () => {
    const [data, setData] = useState({ totalRevenue: 0, totalOrders: 0, newCustomers: 0 })

    useEffect(() => {
        Promise.all([
            getTotalRevenue(),
            getTotalOrders(),
            getNewCustomers()
        ]).then(([rev, ord, cust]) => {

            setData({
                totalRevenue: rev.dt?.totalRevenue ?? 0,
                totalOrders: ord.dt?.totalOrders ?? 0,
                newCustomers: cust.dt?.newCustomers ?? 0
            })

        })
    }, [])

    console.log('KpiCards data:', data) // Log the data to check its structure
    return (
        <div className="kpi-grid">
            <div className="kpi-card">
                <div className="kpi-label">Tổng doanh thu</div>
                <div className="kpi-value">{data.totalRevenue.toLocaleString()}đ</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Tổng hóa đơn</div>
                <div className="kpi-value">{data.totalOrders.toLocaleString()}</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Khách hàng mới</div>
                <div className="kpi-value">+{data.newCustomers}</div>
            </div>
        </div>
    )
}

export default KpiCards
