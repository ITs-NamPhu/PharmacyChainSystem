from app.tools.inventory_tools import create_inventory_tools
from app.tools.sales_tools import create_sales_tools
from app.tools.customer_tools import create_customer_tools
from app.tools.branch_tools import create_branch_tools
from app.tools.supplier_tools import create_supplier_tools
from app.tools.goods_receipt_tools import create_goods_receipt_tools
from app.tools.analytics_tools import create_analytics_tools
from app.tools.forecast_tools import create_forecast_tools

from app.models.chat import AuthContext


def create_all_tools(auth: AuthContext) -> list:
    tools = []
    tools.extend(create_inventory_tools(auth))
    tools.extend(create_sales_tools(auth))
    tools.extend(create_customer_tools(auth))
    tools.extend(create_branch_tools(auth))
    tools.extend(create_supplier_tools(auth))
    tools.extend(create_goods_receipt_tools(auth))
    tools.extend(create_analytics_tools(auth))
    return tools