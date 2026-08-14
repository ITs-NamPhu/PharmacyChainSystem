import { BrowserRouter, Routes, Route } from 'react-router-dom';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer, Toast, Bounce } from "react-toastify";
import { Suspense } from 'react';

import PrivateRoute from './routes/PrivateRoute';
import ComponentNotFound from './components/ComponentNotFound';
import Login from './components/Authentication/Login';


import RoleRoute from './routes/RoleRoute';

import Administration from './components/Management/Administration';

import { ROUTE_PERMISSIONS } from './config/permissions';

import DashBoard from './components/DashBoard/DashBoard';
import IndexManageUser from './components/ManageUser/IndexManageUser';
import IndexManageBranch from './components/ManageBranch/IndexManageBranch';
import IndexManageCustomer from './components/ManageCustomer/IndexManageCustomer';
import IndexManageManufacturer from './components/ManageManufacturer/IndexManageManufacturer';
import IndexManageSupply from './components/ManageSupply/IndexManageSupply';
import IndexManageUnit from './components/ManageUnit/IndexManageUnit';
import IndexManageMedicine from './components/ManageMedicine/IndexManageMedicine';
import IndexManageInvoice from './components/ManageInvoice/IndexManageInvoice';
import IndexManageGoodsReceipt from './components/ManageGoodsReceipt/IndexManageGoodsReceipt';
import IndexManageWarehouse_Medicine from './components/ManageWarehouse_Medicine/IndexManageWarehouse_Medicine';
import IndexStockTake from './components/ManageWareHouse/ManageStockTake/IndexManageStockTake';
import IndexStockTakeAdjustment from './components/ManageWareHouse/ManageStockTakeAdjustment/IndexManageStockTakeAdjustment';
import IndexConfig from './components/ManageConfig/IndexConfig';

const Layout = (props) => {
    return (
        <Suspense fallback={<div>Loading...</div>}>
            <Routes>
                <Route path='/admin' element={
                    <PrivateRoute>
                        <Administration />
                    </PrivateRoute>
                }>
                    <Route index element={<RoleRoute roles={ROUTE_PERMISSIONS.dashboard}><DashBoard /></RoleRoute>} />
                    <Route path='manageuser'
                        element={
                            <RoleRoute roles={ROUTE_PERMISSIONS.manageuser}>
                                <IndexManageUser />
                            </RoleRoute>
                        } />
                    <Route path='managebranch'
                        element={
                            <RoleRoute roles={ROUTE_PERMISSIONS.managebranch}>
                                <IndexManageBranch />
                            </RoleRoute>
                        } />
                    <Route path='managecustomer' element={<RoleRoute roles={ROUTE_PERMISSIONS.managecustomer}><IndexManageCustomer /></RoleRoute>} />
                    <Route path='managemanufacturer' element={<RoleRoute roles={ROUTE_PERMISSIONS.managemanufacturer}><IndexManageManufacturer /></RoleRoute>} />
                    <Route path='managesupply' element={<RoleRoute roles={ROUTE_PERMISSIONS.managesupply}><IndexManageSupply /></RoleRoute>} />
                    <Route path='manageunit' element={<RoleRoute roles={ROUTE_PERMISSIONS.manageunit}><IndexManageUnit /></RoleRoute>} />
                    <Route path='managemedicine' element={<RoleRoute roles={ROUTE_PERMISSIONS.managemedicine}><IndexManageMedicine /></RoleRoute>} />
                    <Route path='manageinvoice' element={<RoleRoute roles={ROUTE_PERMISSIONS.manageinvoice}><IndexManageInvoice /></RoleRoute>} />
                    <Route path='managegoodsreceipt' element={<RoleRoute roles={ROUTE_PERMISSIONS.managegoodsreceipt}><IndexManageGoodsReceipt /></RoleRoute>} />
                    <Route path='managewarehouse_medicine' element={<RoleRoute roles={ROUTE_PERMISSIONS.managewarehouse_medicine}><IndexManageWarehouse_Medicine /></RoleRoute>} />
                    <Route path='managestocktake' element={<RoleRoute roles={ROUTE_PERMISSIONS.managestocktake}><IndexStockTake /></RoleRoute>} />
                    <Route path='managestockadjustment' element={<RoleRoute roles={ROUTE_PERMISSIONS.managestockadjustment}><IndexStockTakeAdjustment /></RoleRoute>} />
                    <Route path='config' element={<RoleRoute roles={ROUTE_PERMISSIONS.config}><IndexConfig /></RoleRoute>} />

                </Route>
                <Route path='/' element={<Login />} />
                <Route path='*' element={<ComponentNotFound />} />

            </Routes>
            <ToastContainer
                position="top-right"
                autoClose={5000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick={false}
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
                theme="dark"
                transition={Bounce}
            />
        </Suspense>
    );
}

export default Layout;
