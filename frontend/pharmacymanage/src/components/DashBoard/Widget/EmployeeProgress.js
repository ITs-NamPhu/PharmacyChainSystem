import { useState, useEffect } from 'react'
import { getEmployeeProgress } from '../../../services/apiService'

const EmployeeProgress = () => {
    const [items, setItems] = useState([])

    useEffect(() => {
        getEmployeeProgress().then(res => {
            setItems(res.data?.DT?.items ?? [])
        })
    }, [])

    return (
        <div className="widget-card">
            <div className="widget-title">Tiến độ nhân viên trong ca</div>
            <div className="widget-table">
                {items.length === 0 && <div className="empty-state">Chưa có dữ liệu</div>}
                {items.map((item, i) => (
                    <div key={i} className="employee-row">
                        <span className="emp-name">{item.userName}</span>
                        <span className="emp-revenue">{item.totalRevenue.toLocaleString()}đ</span>
                        <span className="emp-orders">({item.totalOrders} HĐ)</span>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default EmployeeProgress
