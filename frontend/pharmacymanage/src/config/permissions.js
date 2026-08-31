export const ROUTE_PERMISSIONS = {
  dashboard: ['admin', 'manage_supply', 'manage_branch', 'user_sales', 'user_warehouse'],
  manageuser: ['admin', 'manage_supply', 'manage_branch'],
  managebranch: ['admin', 'manage_supply'],
  managecustomer: ['admin', 'manage_supply', 'manage_branch', 'user_sales', 'user_warehouse'],
  managemanufacturer: ['admin', 'manage_supply'],
  managesupply: ['admin', 'manage_supply'],
  manageunit: ['admin', 'manage_supply'],
  managemedicine: ['admin', 'manage_supply'],
  manageinvoice: ['admin', 'manage_supply', 'manage_branch', 'user_sales'],
  managereceipt: ['admin', 'manage_supply', 'manage_branch', 'user_sales'],
  managegoodsreceipt: ['admin', 'manage_supply', 'manage_branch', 'user_warehouse'],
  managewarehouse_medicine: ['admin', 'manage_supply', 'manage_branch', 'user_sales', 'user_warehouse'],
  managestocktake: ['admin', 'manage_supply'],
  managestockadjustment: ['admin', 'manage_supply'],
  managedestroy: ['admin', 'manage_supply'],
  config: ['admin', 'manage_supply', 'manage_branch', 'user_sales', 'user_warehouse'],
}

export const canAccess = (routeName, roleName) =>
  ROUTE_PERMISSIONS[routeName]?.includes(roleName) ?? false
