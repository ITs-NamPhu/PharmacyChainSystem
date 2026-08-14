import { useState, useEffect } from 'react'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts'
import { getRevenueTrend } from '../../../services/apiService'

const RevenueTrendChart = () => {
    const [data, setData] = useState([])

    useEffect(() => {
        getRevenueTrend(7).then(res => {
            const raw = res.dt?.items ?? []
            setData(raw.map(p => ({
                date: new Date(p.date).toLocaleDateString('vi-VN', { weekday: 'short', day: 'numeric' }),
                revenue: p.revenue
            })))
        })
    }, [])

    return (
        <div className="widget-card chart-card">
            <div className="widget-title">Xu hướng Doanh thu 7 ngày</div>
            <ResponsiveContainer width="100%" height={250}>
                <LineChart data={data}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#eee" />
                    <XAxis dataKey="date" tick={{ fontSize: 11 }} />
                    <YAxis tick={{ fontSize: 11 }} />
                    <Tooltip />
                    <Line type="monotone" dataKey="revenue" stroke="#4f46e5" strokeWidth={2} dot={{ r: 3 }} />
                </LineChart>
            </ResponsiveContainer>
        </div>
    )
}

export default RevenueTrendChart
