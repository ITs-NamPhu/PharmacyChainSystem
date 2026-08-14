import KpiCards from './Widget/KpiCards'
import RevenueTrendChart from './Widget/RevenueTrendChart'
import RevenueByBranchChart from './Widget/RevenueByBranchChart'
import TopMedicinesChart from './Widget/TopMedicinesChart'
import ExpiringBatches from './Widget/ExpiringBatches'
import LowStockAlerts from './Widget/LowStockAlerts'
import DestroyQueue from './Widget/DestroyQueue'
import SupplyKpi from './Widget/SupplyKpi'
import StockTransferRequests from './Widget/StockTransferRequests'
import BranchKpi from './Widget/BranchKpi'
import OutOfStockList from './Widget/OutOfStockList'
import EmployeeProgress from './Widget/EmployeeProgress'
import ShiftKpi from './Widget/ShiftKpi'
import PromotionsWidget from './Widget/PromotionsWidget'
import CounterAlerts from './Widget/CounterAlerts'

export const AdminLayout = () => (
    <div className="dashboard-layout">
        <div className="kpi-row">
            <KpiCards />
        </div>
        <div className="chart-row">
            <RevenueTrendChart />
        </div>
        <div className="split-row">
            <RevenueByBranchChart />
            <TopMedicinesChart />
        </div>
    </div>
)

export const SupplyLayout = () => (
    <div className="dashboard-layout">
        <div className="kpi-row">
            <SupplyKpi />
        </div>
        <div className="chart-row">
            <StockTransferRequests />
        </div>
        <div className="split-row">
            <ExpiringBatches />
            <DestroyQueue />
        </div>
    </div>
)

export const BranchLayout = () => (
    <div className="dashboard-layout">
        <div className="kpi-row">
            <BranchKpi />
        </div>
        <div className="chart-row">
            <RevenueTrendChart />
        </div>
        <div className="split-row">
            <EmployeeProgress />
            <div className="stack-col">
                <LowStockAlerts />
                <OutOfStockList />
            </div>
        </div>
    </div>
)

export const WarehouseLayout = () => (
    <div className="dashboard-layout">
        <div className="kpi-row">
            <LowStockAlerts />
        </div>
        <div className="chart-row">
            <ExpiringBatches />
        </div>
        <div className="split-row">
            <OutOfStockList />
            <DestroyQueue />
        </div>
    </div>
)

export const SalesLayout = () => (
    <div className="dashboard-layout">
        <div className="kpi-row">
            <ShiftKpi />
        </div>
        <div className="pos-button-row">
            <button className="pos-btn" onClick={() => window.open('/admin/manageinvoice', '_self')}>
                MỞ MÀN HÌNH BÁN HÀNG (POS)
            </button>
        </div>
        <div className="split-row">
            <PromotionsWidget />
            <CounterAlerts />
        </div>
    </div>
)
