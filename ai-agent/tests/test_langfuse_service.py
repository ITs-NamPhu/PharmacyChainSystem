import pytest

from app.config.settings import settings
from app.services import langfuse_service


class FakeClient:
    def __init__(self, auth_result: bool = True):
        self.auth_result = auth_result
        self.flush_count = 0
        self.shutdown_count = 0

    def auth_check(self) -> bool:
        return self.auth_result

    def flush(self) -> None:
        self.flush_count += 1

    def shutdown(self) -> None:
        self.shutdown_count += 1


@pytest.fixture(autouse=True)
def reset_client():
    langfuse_service._client = None
    yield
    langfuse_service._client = None


def _enable_settings(monkeypatch):
    monkeypatch.setattr(settings, "LANGFUSE_PUBLIC_KEY", "pk-lf")
    monkeypatch.setattr(settings, "LANGFUSE_SECRET_KEY", "sk-lf")
    monkeypatch.setattr(settings, "LANGFUSE_BASE_URL", "http://langfuse:3000")


def test_settings_exposes_base_url():
    assert settings.LANGFUSE_BASE_URL


def test_init_returns_none_without_keys(monkeypatch):
    monkeypatch.setattr(settings, "LANGFUSE_PUBLIC_KEY", None)
    monkeypatch.setattr(settings, "LANGFUSE_SECRET_KEY", None)

    assert langfuse_service.init_langfuse_client() is None
    assert langfuse_service.get_langfuse_client() is None


def test_init_creates_single_client_with_base_url(monkeypatch):
    _enable_settings(monkeypatch)
    captured = {}

    def factory(**kwargs):
        captured.update(kwargs)
        return FakeClient()

    monkeypatch.setattr(langfuse_service, "Langfuse", factory)

    client = langfuse_service.init_langfuse_client()
    assert client is not None
    assert captured["public_key"] == "pk-lf"
    assert captured["secret_key"] == "sk-lf"
    assert captured["base_url"] == "http://langfuse:3000"
    assert langfuse_service.get_langfuse_client() is client
    assert langfuse_service.init_langfuse_client() is client


def test_init_returns_none_when_auth_check_fails(monkeypatch):
    _enable_settings(monkeypatch)
    client = FakeClient(auth_result=False)
    monkeypatch.setattr(langfuse_service, "Langfuse", lambda **kwargs: client)

    assert langfuse_service.init_langfuse_client() is None
    assert langfuse_service.get_langfuse_client() is None
    assert client.shutdown_count == 1


def test_init_does_not_raise_when_langfuse_unreachable(monkeypatch):
    _enable_settings(monkeypatch)
    monkeypatch.setattr(langfuse_service, "_INIT_MAX_ATTEMPTS", 1)
    monkeypatch.setattr(langfuse_service, "_INIT_RETRY_DELAY_SECONDS", 0)

    def factory(**kwargs):
        raise RuntimeError("connection refused")

    monkeypatch.setattr(langfuse_service, "Langfuse", factory)

    assert langfuse_service.init_langfuse_client() is None
    assert langfuse_service.get_langfuse_client() is None


def test_init_retries_before_giving_up(monkeypatch):
    _enable_settings(monkeypatch)
    monkeypatch.setattr(langfuse_service, "_INIT_MAX_ATTEMPTS", 3)
    monkeypatch.setattr(langfuse_service, "_INIT_RETRY_DELAY_SECONDS", 0)
    calls = []

    def factory(**kwargs):
        calls.append(kwargs)
        if len(calls) < 3:
            raise RuntimeError("connection refused")
        return FakeClient()

    monkeypatch.setattr(langfuse_service, "Langfuse", factory)

    client = langfuse_service.init_langfuse_client()
    assert client is not None
    assert len(calls) == 3


def test_flush_and_shutdown_are_safe_without_client():
    langfuse_service.flush_langfuse_client()
    langfuse_service.shutdown_langfuse_client()


def test_shutdown_flushes_before_clearing_client():
    client = FakeClient()
    langfuse_service._client = client

    langfuse_service.flush_langfuse_client()
    assert client.flush_count == 1

    langfuse_service.shutdown_langfuse_client()
    assert client.flush_count == 2
    assert client.shutdown_count == 1
    assert langfuse_service.get_langfuse_client() is None
