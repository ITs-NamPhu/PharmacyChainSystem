import { useState, useEffect } from 'react'
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend } from 'recharts'
import { getRevenueByBranch } from '../../../services/apiService'

const COLORS = ['#4f46e5', '#06b6d4', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']

const RevenueByBranchChart = () => {
    const [data, setData] = useState([])

    useEffect(() => {
        getRevenueByBranch().then(res => {
            const raw = res.dt?.items ?? []
            setData(raw.map(i => ({ name: i.branchName, value: i.revenue, pct: i.percentage })))
        })
    }, [])

    return (
        <div className="widget-card chart-card">
            <div className="widget-title">Doanh thu theo chi nhánh</div>
            <ResponsiveContainer width="100%" height={250}>
                <PieChart>
                    <Pie data={data} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={80} label={({ name, pct }) => `${name}: ${pct}%`}>
                        {data.map((_, i) => (
                            <Cell key={i} fill={COLORS[i % COLORS.length]} />
                        ))}
                    </Pie>
                    <Tooltip />
                    <Legend />
                </PieChart>
            </ResponsiveContainer>
        </div>
    )
}

export default RevenueByBranchChart
