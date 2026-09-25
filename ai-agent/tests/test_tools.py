import json

import pytest

from app.services.backend_client import BackendError
from app.tools import create_all_tools


@pytest.fixture
def tools(auth):
    return {t.name: t for t in create_all_tools(auth)}


def _last_call(backend_stub):
    assert backend_stub.calls, "no backend call recorded"
    return backend_stub.calls[-1]


@pytest.mark.asyncio
async def test_analytics_happy_path(tools, backend_stub):
    backend_stub.set_response("/api/ai/analytics/report", {"Items": [{"name": "A"}], "Total": 1})
    out = await tools["analytics_report_tool"].ainvoke(
        {"report_type": "TopSellingMedicine", "top_n": 1}
    )
    call = _last_call(backend_stub)
    assert call["path"] == "/api/ai/analytics/report"
    assert call["json_body"] == {"ReportType": "TopSellingMedicine", "TopN": 1}
    assert call["token"] == "test-token"
    assert call["branch_id"] == "1"
    assert json.loads(out)["Total"] == 1


@pytest.mark.asyncio
async def test_analytics_invalid_report_type(tools, backend_stub):
    out = await tools["analytics_report_tool"].ainvoke({"report_type": "Whatever"})
    assert "khong hop le" in out
    assert backend_stub.calls == []


@pytest.mark.asyncio
async def test_analytics_empty_items(tools, backend_stub):
    backend_stub.set_response("/api/ai/analytics/report", {"Items": []})
    out = await tools["analytics_report_tool"].ainvoke({"report_type": "TotalRevenue"})
    assert "Khong tim thay du lieu" in out


@pytest.mark.asyncio
async def test_search_inventory_payload_mapping(tools, backend_stub):
    backend_stub.set_response(
        "/api/ai/inventory/search",
        {"Items": [{"MedicineId": 1, "MedicineName": "Paracetamol"}]},
    )
    out = await tools["search_inventory"].ainvoke(
        {"keyword": "para", "is_low_stock": True, "stock_threshold": 5, "count": 10}
    )
    call = _last_call(backend_stub)
    assert call["path"] == "/api/ai/inventory/search"
    assert call["json_body"] == {
        "Keyword": "para", "IsLowStock": True, "StockThreshold": 5, "Count": 10,
    }
    assert "Paracetamol" in out


@pytest.mark.asyncio
async def test_search_inventory_empty(tools, backend_stub):
    backend_stub.set_response("/api/ai/inventory/search", {"Items": []})
    out = await tools["search_inventory"].ainvoke({"keyword": "zzz"})
    assert "Khong tim thay thuoc" in out


@pytest.mark.asyncio
async def test_inventory_alerts_low_stock(tools, backend_stub):
    backend_stub.set_response("/api/ai/inventory/search", {"Items": [{"MedicineId": 2}]})
    await tools["inventory_alerts"].ainvoke({"alert_type": "low_stock", "threshold": 3})
    call = _last_call(backend_stub)
    assert call["json_body"] == {"IsLowStock": True, "StockThreshold": 3}


@pytest.mark.asyncio
async def test_inventory_alerts_invalid(tools, backend_stub):
    out = await tools["inventory_alerts"].ainvoke({"alert_type": "bogus"})
    assert "khong hop le" in out
    assert backend_stub.calls == []


@pytest.mark.asyncio
async def test_allocate_fefo(tools, backend_stub):
    backend_stub.set_response("/api/Invoice/FefoBatches", [{"BatchNo": "L1"}])
    out = await tools["allocate_fefo"].ainvoke({"medicine_id": 5, "quantity": 3})
    call = _last_call(backend_stub)
    assert call["method"] == "GET"
    assert call["path"] == "/api/Invoice/FefoBatches"
    assert call["params"] == {"medicineID": 5, "quantity": 3}
    assert "L1" in out


@pytest.mark.asyncio
async def test_search_sales_payment_status_paid(tools, backend_stub):
    backend_stub.set_response("/api/ai/sales/search", {"Items": []})
    await tools["search_sales"].ainvoke({"payment_status": "paid", "customer_id": 9})
    call = _last_call(backend_stub)
    assert call["json_body"]["PaymentStatus"] == 1
    assert call["json_body"]["CustomerId"] == 9


@pytest.mark.asyncio
async def test_search_sales_payment_status_debt(tools, backend_stub):
    backend_stub.set_response("/api/ai/sales/search", {"Items": []})
    await tools["search_sales"].ainvoke({"payment_status": "debt"})
    assert _last_call(backend_stub)["json_body"]["PaymentStatus"] == 0


@pytest.mark.asyncio
async def test_search_sales_invalid_payment_status(tools, backend_stub):
    out = await tools["search_sales"].ainvoke({"payment_status": "nope"})
    assert "khong hop le" in out
    assert backend_stub.calls == []


@pytest.mark.asyncio
async def test_search_customers(tools, backend_stub):
    backend_stub.set_response(
        "/api/ai/customer/search", {"Items": [{"CustomerId": 1, "CustomerName": "Nguyen A"}]}
    )
    out = await tools["search_customers"].ainvoke({"has_active_debt": True, "keyword": "Ng"})
    call = _last_call(backend_stub)
    assert call["json_body"] == {"HasActiveDebt": True, "Keyword": "Ng"}
    assert "Nguyen A" in out


@pytest.mark.asyncio
async def test_search_goods_receipts(tools, backend_stub):
    backend_stub.set_response(
        "/api/ai/goodsreceipt/search",
        {"Items": [{"GoodsReceiptId": 11, "ReceiptNumber": "PN001"}]},
    )
    out = await tools["search_goods_receipts"].ainvoke(
        {"status": "APPROVED", "from_date": "2026-09-01", "to_date": "2026-09-30"}
    )
    call = _last_call(backend_stub)
    assert call["json_body"]["Status"] == "APPROVED"
    assert call["json_body"]["FromDate"] == "2026-09-01"
    assert "PN001" in out


@pytest.mark.asyncio
async def test_list_branches(admin_auth, backend_stub):
    tools = {t.name: t for t in create_all_tools(admin_auth)}
    backend_stub.set_response("/api/Branch/All", [{"BranchID": 1, "BranchName": "HN"}])
    out = await tools["list_branches"].ainvoke({})
    assert "HN" in out
    call = _last_call(backend_stub)
    assert call["branch_id"] is None


@pytest.mark.asyncio
async def test_search_supplier_filters_locally(tools, backend_stub):
    backend_stub.set_response(
        "/api/Supplier/GetAll",
        {"Suppliers": [{"SupplierName": "Duoc Hau Giang"}, {"SupplierName": "Duoc Mekophar"}]},
    )
    out = await tools["search_supplier"].ainvoke({"query": "Hau Giang"})
    assert "Hau Giang" in out
    assert "Mekophar" not in out


@pytest.mark.asyncio
async def test_search_supplier_empty(tools, backend_stub):
    backend_stub.set_response("/api/Supplier/GetAll", {"Suppliers": []})
    out = await tools["search_supplier"].ainvoke({"query": ""})
    assert "Khong tim thay nha cung cap" in out


@pytest.mark.asyncio
async def test_backend_error_is_returned_as_string(tools, monkeypatch):
    from app.services.backend_client import backend_client

    async def _fail(path, json_body=None, token=None, branch_id=None):
        raise BackendError("EC=-999 token expired", status_code=200, ec=-999)

    monkeypatch.setattr(backend_client, "post", _fail)
    out = await tools["search_inventory"].ainvoke({"keyword": "x"})
    assert "EC=-999" in out