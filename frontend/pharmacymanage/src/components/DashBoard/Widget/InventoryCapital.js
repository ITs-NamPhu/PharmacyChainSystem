import { useState, useEffect } from 'react'
import { getInventoryCapital } from '../../../services/apiService'

const InventoryCapital = () => {
    const [value, setValue] = useState(0)

    useEffect(() => {
        getInventoryCapital().then(res => {
            setValue(res.data?.DT?.totalCapital ?? 0)
        })
    }, [])

    return (
        <div className="kpi-card">
            <div className="kpi-label">Tổng vốn tồn kho</div>
            <div className="kpi-value">{value.toLocaleString()}đ</div>
        </div>
    )
}

export default InventoryCapital
