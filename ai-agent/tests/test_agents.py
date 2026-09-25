import re

import pytest
from langchain_core.messages import AIMessage

from app.agents.pharmacy_agent import (
    build_agent_callbacks,
    build_langfuse_metadata,
    create_pharmacy_agent,
)
from app.services import llm_service
from app.tools import create_all_tools
from tests.conftest import make_fake_llm


def test_create_all_tools_unique_and_complete(auth):
    tools = create_all_tools(auth)
    names = [t.name for t in tools]
    assert len(names) == len(set(names)), "tool names must be unique"
    assert len(tools) >= 9
    for expected in (
        "search_inventory", "inventory_alerts", "allocate_fefo",
        "search_sales", "search_customers", "list_branches",
        "search_supplier", "search_goods_receipts", "analytics_report_tool",
    ):
        assert expected in names


def test_create_pharmacy_agent_with_fake_llm(auth):
    llm = make_fake_llm([AIMessage(content="xin chao")])
    agent = create_pharmacy_agent("s1", auth, llm=llm)
    assert hasattr(agent, "astream_events")
    assert hasattr(agent, "get_graph")


def test_build_agent_callbacks_disabled_without_client(monkeypatch):
    from app.services import langfuse_service

    monkeypatch.setattr(langfuse_service, "get_langfuse_client", lambda: None)
    assert build_agent_callbacks() == []


def test_build_agent_callbacks_returns_handler_when_client_ready(monkeypatch):
    import langfuse.langchain as langfuse_langchain
    from app.services import langfuse_service

    class FakeHandler:
        pass

    monkeypatch.setattr(langfuse_service, "get_langfuse_client", lambda: object())
    monkeypatch.setattr(langfuse_langchain, "CallbackHandler", FakeHandler)

    callbacks = build_agent_callbacks()
    assert len(callbacks) == 1
    assert isinstance(callbacks[0], FakeHandler)


def test_build_langfuse_metadata_sets_session_and_trace_name():
    md = build_langfuse_metadata("90002")
    assert md is not None
    assert md["langfuse_session_id"] == "90002"
    assert md["langfuse_trace_name"].endswith("90002")
    assert "langfuse_user_id" not in md


def test_build_langfuse_metadata_adds_branch_user_id(auth):
    md = build_langfuse_metadata("90002", auth)
    assert md is not None
    assert md["langfuse_user_id"] == f"branch:{auth.branch_id}"


def test_build_langfuse_metadata_none_without_session():
    assert build_langfuse_metadata(None) is None
    assert build_langfuse_metadata("") is None


# ---------- llm_service / routing ----------

def test_route_query_simple_lookup_is_groq():
    assert llm_service.route_query("thuoc paracetamol con hang khong") == "groq"


def test_route_query_complex_prefers_gemini_when_available(monkeypatch):
    class FakeSettings:
        effective_gemini_api_key = "fake-key"

    monkeypatch.setattr(llm_service, "settings", FakeSettings())
    assert llm_service.route_query("thuoc nao ban chay nhat trong thang nay") == "gemini"


def test_route_query_complex_falls_back_groq_without_key():
    # Không set gemini key trong test env -> phải về groq
    if llm_service.settings.effective_gemini_api_key:
        pytest.skip("GEMINI_API_KEY present in env, can't test fallback here")
    assert llm_service.route_query("tong doanh thu thang nay") == "groq"


def test_create_groq_llm_requires_key(monkeypatch):
    from app.config.settings import settings
    monkeypatch.setattr(settings, "GROQ_API_KEY", None)
    with pytest.raises(ValueError, match="GROQ_API_KEY"):
        llm_service.create_groq_llm()


def test_create_llm_fallback_for_unknown_provider(monkeypatch):
    from app.config.settings import settings
    monkeypatch.setattr(settings, "GROQ_API_KEY", None)
    with pytest.raises(ValueError):
        # unknown provider logs warning và fallback groq, groq thiếu key -> raise
        llm_service.create_llm("unknown")


def test_create_llm_for_query_auto_uses_router(monkeypatch):
    from app.config.settings import settings
    monkeypatch.setattr(settings, "GROQ_API_KEY", None)
    with pytest.raises(ValueError):
        llm_service.create_llm_for_query("thuoc X con khong", "auto")