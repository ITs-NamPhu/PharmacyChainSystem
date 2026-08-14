import { useState, useEffect } from 'react'
import { getPendingImports } from '../../../services/apiService'

const PendingImports = () => {
    const [count, setCount] = useState(0)

    useEffect(() => {
        getPendingImports().then(res => {
            setCount(res.data?.DT?.pendingCount ?? 0)
        })
    }, [])

    return (
        <div className="kpi-card">
            <div className="kpi-label">Đơn nhập chờ duyệt</div>
            <div className="kpi-value">{count}</div>
        </div>
    )
}

export default PendingImports
