import httpx
import pytest
import respx

from app.services.backend_client import BackendClient, BackendError


@pytest.fixture
def client():
    # Client riêng (không dùng singleton) để tránh nhiễu trạng thái token_expired
    return BackendClient(base_url="http://dg-test")


@pytest.mark.asyncio
async def test_get_returns_dt(client):
    with respx.mock() as router:
        router.get("http://dg-test/api/x").mock(
            return_value=httpx.Response(200, json={"EC": 0, "DT": {"id": 1}, "EM": "ok"})
        )
        data = await client.get("/api/x")
    assert data == {"id": 1}


@pytest.mark.asyncio
async def test_backend_error_raises_and_flags_expired(client):
    with respx.mock() as router:
        router.post("http://dg-test/api/x").mock(
            return_value=httpx.Response(200, json={"EC": -999, "EM": "token expired"})
        )
        with pytest.raises(BackendError) as exc:
            await client.post("/api/x", token="abc")
        assert exc.value.ec == -999
        assert client.token_expired is True


@pytest.mark.asyncio
async def test_ec_error_raises_without_expiry_flag(client):
    with respx.mock() as router:
        router.post("http://dg-test/api/x").mock(
            return_value=httpx.Response(200, json={"EC": 2, "EM": "bad data"})
        )
        with pytest.raises(BackendError) as exc:
            await client.post("/api/x")
        assert exc.value.ec == 2
        assert client.token_expired is False


@pytest.mark.asyncio
async def test_non_json_response_raises(client):
    with respx.mock() as router:
        router.get("http://dg-test/api/x").mock(
            return_value=httpx.Response(500, text="<html>oops</html>")
        )
        with pytest.raises(BackendError) as exc:
            await client.get("/api/x")
        assert "HTTP 500" in str(exc.value)


@pytest.mark.asyncio
async def test_http_error_without_ec_raises(client):
    with respx.mock() as router:
        router.get("http://dg-test/api/x").mock(
            return_value=httpx.Response(404, json={"EM": "not found"})
        )
        with pytest.raises(BackendError) as exc:
            await client.get("/api/x")
        assert exc.value.status_code == 404


@pytest.mark.asyncio
async def test_authorization_header_adds_bearer_prefix(client):
    captured = {}

    def _capture(request):
        captured["authorization"] = request.headers.get("authorization")
        captured["x_branch"] = request.headers.get("x-branch-id")
        return httpx.Response(200, json={"EC": 0, "DT": {}})

    with respx.mock() as router:
        router.get("http://dg-test/api/x").mock(side_effect=_capture)
        await client.get("/api/x", token="plain-token", branch_id="7")
    assert captured["authorization"] == "Bearer plain-token"
    assert captured["x_branch"] == "7"


@pytest.mark.asyncio
async def test_get_shortcut_and_dt_key(client):
    with respx.mock() as router:
        router.get("http://dg-test/api/list").mock(
            return_value=httpx.Response(200, json={"dt": [1, 2, 3]})
        )
        data = await client.get("/api/list")
    assert data == [1, 2, 3]