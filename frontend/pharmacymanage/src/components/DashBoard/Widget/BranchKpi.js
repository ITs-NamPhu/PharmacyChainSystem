import { useState, useEffect } from 'react'
import { getBranchRevenue, getBranchOrders, getOutOfStock, getExpiringBatches } from '../../../services/apiService'

const BranchKpi = () => {
    const [data, setData] = useState({ revenue: 0, orders: 0, outOfStock: 0, nearExpiry: 0 })

    useEffect(() => {
        Promise.all([
            getBranchRevenue(),
            getBranchOrders(),
            getOutOfStock(),
            getExpiringBatches()
        ]).then(([rev, ord, oos, exp]) => {
            setData({
                revenue: rev.data?.DT?.totalRevenue ?? 0,
                orders: ord.data?.DT?.totalOrders ?? 0,
                outOfStock: oos.data?.DT?.items?.length ?? 0,
                nearExpiry: exp.data?.DT?.items?.length ?? 0
            })
        })
    }, [])

    return (
        <div className="kpi-grid">
            <div className="kpi-card">
                <div className="kpi-label">Doanh thu chi nhánh</div>
                <div className="kpi-value">{data.revenue.toLocaleString()}đ</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Hóa đơn</div>
                <div className="kpi-value">{data.orders}</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Thuốc hết hàng</div>
                <div className="kpi-value">{data.outOfStock}</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Cận date</div>
                <div className="kpi-value">{data.nearExpiry}</div>
            </div>
        </div>
    )
}

export default BranchKpi
