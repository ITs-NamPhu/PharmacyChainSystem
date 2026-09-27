from typing import Iterator, List

import pytest
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.language_models.fake_chat_models import GenericFakeChatModel
from langchain_core.messages import BaseMessage

from app.models.chat import AuthContext
from app.services.backend_client import backend_client


class FakeToolLlm(GenericFakeChatModel):
    """GenericFakeChatModel hỗ trợ bind_tools (cần thiết cho create_react_agent)."""

    def bind_tools(self, tools, **kwargs):
        return self


def make_fake_llm(messages: List[BaseMessage]) -> FakeToolLlm:
    """Tạo fake LLM trả các message lần lượt (dùng chung cho nhiều turn)."""
    return FakeToolLlm(messages=iter(messages))


@pytest.fixture
def auth() -> AuthContext:
    return AuthContext(token="test-token", branch_id="1")


@pytest.fixture
def admin_auth() -> AuthContext:
    # Admin: không giới hạn chi nhánh
    return AuthContext(token="test-token", branch_id=None)


@pytest.fixture
def fake_llm():
    """Fake LLM mặc định trả lời văn bản (không gọi tool)."""

    def _factory(messages: List[BaseMessage]):
        return make_fake_llm(messages)

    return _factory


class _BackendStub:
    """Stub cho backend_client: ghi lại các lời gọi để assert."""

    def __init__(self):
        self.calls: List[dict] = []
        self.responses: dict = {}
        self.default_response = None

    def set_response(self, path: str, response):
        self.responses[path] = response

    async def get(self, path, params=None, token=None, branch_id=None):
        self.calls.append({"method": "GET", "path": path, "params": params,
                           "token": token, "branch_id": branch_id})
        return self.responses.get(path, self.default_response)

    async def post(self, path, json_body=None, token=None, branch_id=None):
        self.calls.append({"method": "POST", "path": path, "json_body": json_body,
                           "token": token, "branch_id": branch_id})
        return self.responses.get(path, self.default_response)


@pytest.fixture
def backend_stub(monkeypatch) -> _BackendStub:
    """Thay backend_client singleton bằng stub ghi lại lời gọi (deterministic)."""
    stub = _BackendStub()
    monkeypatch.setattr(backend_client, "get", stub.get)
    monkeypatch.setattr(backend_client, "post", stub.post)
    return stub


@pytest.fixture
def mock_http():
    """Cài respx để chặn HTTP thật ở tầng client (dùng cho test backend_client.request)."""
    import respx

    with respx.mock(assert_all_called=False) as router:
        yield router


class StubRouterLLM:
    """LLM giả cho router: with_structured_output trả RunnableLambda cố định.

    GenericFakeChatModel không hỗ trợ with_structured_output nên cần lớp riêng.
    """

    def __init__(self, next_agent: str = "chat_agent", should_raise: bool = False):
        from app.agents.router import RouteDecision

        self.decision = RouteDecision(next_agent=next_agent)
        self.should_raise = should_raise
        self.captured: dict = {}

    def bind_tools(self, tools, **kwargs):
        return self

    def with_structured_output(self, schema, **kwargs):
        from langchain_core.runnables import RunnableLambda

        async def _run(prompt_value, config=None):
            self.captured["config"] = config
            if self.should_raise:
                raise ValueError("structured output hong")
            return self.decision

        return RunnableLambda(_run)


def make_router_llm(next_agent: str = "chat_agent", should_raise: bool = False) -> StubRouterLLM:
    return StubRouterLLM(next_agent=next_agent, should_raise=should_raise)