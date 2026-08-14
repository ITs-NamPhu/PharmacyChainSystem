import { useState, useEffect } from 'react'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import { getTopMedicines } from '../../../services/apiService'

const TopMedicinesChart = () => {
    const [data, setData] = useState([])

    useEffect(() => {
        getTopMedicines(10).then(res => {
            const raw = res.dt?.items ?? []
            setData(raw.map(i => ({ name: i.medicineName, qty: i.totalQuantity, revenue: i.totalRevenue })))
        })
    }, [])

    return (
        <div className="widget-card chart-card">
            <div className="widget-title">Top 10 thuốc bán chạy</div>
            <ResponsiveContainer width="100%" height={250}>
                <BarChart data={data} layout="vertical" margin={{ left: 100 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#eee" />
                    <XAxis type="number" tick={{ fontSize: 11 }} />
                    <YAxis type="category" dataKey="name" tick={{ fontSize: 10 }} width={90} />
                    <Tooltip />
                    <Bar dataKey="qty" fill="#4f46e5" radius={[0, 4, 4, 0]} />
                </BarChart>
            </ResponsiveContainer>
        </div>
    )
}

export default TopMedicinesChart
