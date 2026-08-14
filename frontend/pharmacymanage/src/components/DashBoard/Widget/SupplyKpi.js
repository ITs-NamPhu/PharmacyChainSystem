import { useState, useEffect } from 'react'
import { getInventoryCapital, getPendingImports, getPendingStockTransfers, getExpiringBatches } from '../../../services/apiService'

const SupplyKpi = () => {
    const [data, setData] = useState({ capital: 0, pendingImports: 0, pendingTransfers: 0, nearExpiry: 0 })

    useEffect(() => {
        Promise.all([
            getInventoryCapital(),
            getPendingImports(),
            getPendingStockTransfers(),
            getExpiringBatches()
        ]).then(([cap, imp, trf, exp]) => {
            setData({
                capital: cap.dt?.totalCapital ?? 0,
                pendingImports: imp.dt?.pendingCount ?? 0,
                pendingTransfers: trf.dt?.pendingCount ?? 0,
                nearExpiry: exp.dt?.items?.length ?? 0
            })
        })
    }, [])

    return (
        <div className="kpi-grid">
            <div className="kpi-card">
                <div className="kpi-label">Tổng vốn tồn kho</div>
                <div className="kpi-value">{data.capital.toLocaleString()}đ</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Đơn nhập chờ duyệt</div>
                <div className="kpi-value">{data.pendingImports}</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Phiếu chuyển kho</div>
                <div className="kpi-value">{data.pendingTransfers}</div>
            </div>
            <div className="kpi-card">
                <div className="kpi-label">Thuốc cận date</div>
                <div className="kpi-value">{data.nearExpiry}</div>
            </div>
        </div>
    )
}

export default SupplyKpi
