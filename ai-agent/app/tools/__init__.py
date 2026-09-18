from app.tools.medicine_tools import create_medicine_tools
from app.tools.warehouse_tools import create_warehouse_tools
from app.tools.invoice_tools import create_invoice_tools
from app.tools.customer_tools import create_customer_tools
from app.tools.dashboard_tools import create_dashboard_tools
from app.tools.branch_tools import create_branch_tools
from app.tools.supplier_tools import create_supplier_tools

from app.models.chat import AuthContext


def create_all_tools(auth: AuthContext) -> list:
    tools = []
    tools.extend(create_medicine_tools(auth))
    tools.extend(create_warehouse_tools(auth))
    tools.extend(create_invoice_tools(auth))
    tools.extend(create_customer_tools(auth))
    tools.extend(create_dashboard_tools(auth))
    tools.extend(create_branch_tools(auth))
    tools.extend(create_supplier_tools(auth))
    return tools
