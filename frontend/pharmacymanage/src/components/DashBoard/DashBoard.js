import { useSelector } from 'react-redux'
import { AdminLayout, SupplyLayout, BranchLayout, SalesLayout, WarehouseLayout } from './DashboardLayouts'
import './DashBoard.scss'

const DashBoard = () => {
    const account = useSelector(state => state.user?.account)
    const roleName = account?.roleName ?? ''
    console.log('DashBoard roleName:', roleName)
    const layoutMap = {
        admin: AdminLayout,
        manage_supply: SupplyLayout,
        manage_branch: BranchLayout,
        user_sale: SalesLayout,
        user_warehouse: WarehouseLayout
    }

    const Layout = layoutMap[roleName] || AdminLayout

    return <Layout />
}

export default DashBoard
