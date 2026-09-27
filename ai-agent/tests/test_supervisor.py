import pytest
from langchain_core.messages import AIMessage, HumanMessage
from langchain_core.runnables import RunnableLambda

from app.agents.router import (
    CHAT_AGENT,
    FORECAST_AGENT,
    ROUTER_TAG,
    RouteDecision,
    build_router_node,
    heuristic_route,
)
from app.agents.state import AgentState
from app.agents.supervisor import create_supervisor_graph
from tests.conftest import make_fake_llm


class _StubStructuredLLM:
    """LLM giả: with_structured_output trả về RunnableLambda cố định."""

    def __init__(self, decision, should_raise=False):
        self.decision = decision
        self.should_raise = should_raise
        self.captured = {}

    def bind_tools(self, tools, **kwargs):
        return self

    def with_structured_output(self, schema, **kwargs):
        async def _run(prompt_value, config=None):
            self.captured["config"] = config
            if self.should_raise:
                raise ValueError("structured output hong")
            return self.decision

        return RunnableLambda(_run)


@pytest.mark.asyncio
async def test_router_routes_forecast_question():
    node = build_router_node(_StubStructuredLLM(RouteDecision(next_agent=FORECAST_AGENT)))
    result = await node({"messages": [HumanMessage("dự báo nhập hàng cho tiffy")]})
    assert result["next_agent"] == FORECAST_AGENT


@pytest.mark.asyncio
async def test_router_routes_chat_question():
    node = build_router_node(_StubStructuredLLM(RouteDecision(next_agent=CHAT_AGENT)))
    result = await node({"messages": [HumanMessage("tồn kho thuốc tiffy còn bao nhiêu?")]})
    assert result["next_agent"] == CHAT_AGENT


@pytest.mark.asyncio
async def test_router_falls_back_to_heuristic_when_llm_fails():
    node = build_router_node(_StubStructuredLLM(None, should_raise=True))
    result = await node({"messages": [HumanMessage("dự báo nhập hàng tuần sau")]})
    assert result["next_agent"] == FORECAST_AGENT


@pytest.mark.asyncio
async def test_router_falls_back_to_chat_on_ambiguous_question():
    node = build_router_node(_StubStructuredLLM(None, should_raise=True))
    result = await node({"messages": [HumanMessage("xin chào, hôm nay thế nào?")]})
    assert result["next_agent"] == CHAT_AGENT


@pytest.mark.asyncio
async def test_router_without_user_message_goes_to_chat():
    node = build_router_node(_StubStructuredLLM(RouteDecision(next_agent=FORECAST_AGENT)))
    result = await node({"messages": [AIMessage("chào")]})
    assert result["next_agent"] == CHAT_AGENT


@pytest.mark.asyncio
async def test_router_tags_its_own_run():
    llm = _StubStructuredLLM(RouteDecision(next_agent=CHAT_AGENT))
    node = build_router_node(llm)
    await node({"messages": [HumanMessage("xin chào")]})
    assert llm.captured["config"]["tags"] == [ROUTER_TAG]


@pytest.mark.parametrize(
    "text,expected",
    [
        ("Dự báo nhập hàng tuần sau", FORECAST_AGENT),
        ("có nên nhập thêm tiffy không", FORECAST_AGENT),
        ("Có nguy cơ thiếu hụt gì không", FORECAST_AGENT),
        ("doanh thu tháng này bao nhiêu", CHAT_AGENT),
        ("thuốc nào tồn kho thấp", CHAT_AGENT),
    ],
)
def test_heuristic_route(text, expected):
    assert heuristic_route(text) == expected


def test_heuristic_route_handles_empty_text():
    assert heuristic_route("") == CHAT_AGENT


def test_agent_state_shape():
    state: AgentState = {"messages": [HumanMessage("hi")], "next_agent": CHAT_AGENT}
    assert state["next_agent"] == CHAT_AGENT


def test_supervisor_graph_compiles_with_all_nodes(auth):
    graph = create_supervisor_graph("s1", auth, llm=make_fake_llm([AIMessage("ok")]))
    assert hasattr(graph, "ainvoke")
    assert hasattr(graph, "astream_events")

    nodes = {n for n in graph.get_graph().nodes if n not in ("__start__", "__end__")}
    assert {"router", "chat_agent", "forecast_agent"} <= nodes


@pytest.mark.asyncio
async def test_supervisor_end_to_end_routes_to_chat_agent(auth):
    graph = create_supervisor_graph(
        "s1",
        auth,
        llm=make_fake_llm([AIMessage("tồn kho còn 15 vỉ")]),
        router_llm=_StubStructuredLLM(RouteDecision(next_agent=CHAT_AGENT)),
    )
    result = await graph.ainvoke(
        {"messages": [HumanMessage("tồn kho tiffy còn bao nhiêu")], "next_agent": ""}
    )
    assert result["next_agent"] == CHAT_AGENT
    assert result["messages"][-1].content == "tồn kho còn 15 vỉ"


@pytest.mark.asyncio
async def test_router_events_are_tagged_in_astream_events(auth):
    """Tag phải nổi lên được trong astream_events, nếu không chat.py lọc rác vô ích."""
    graph = create_supervisor_graph(
        "s1",
        auth,
        llm=make_fake_llm([AIMessage("câu trả lời")]),
        router_llm=_StubStructuredLLM(RouteDecision(next_agent=CHAT_AGENT)),
    )

    router_events = []
    answer_events = []
    async for event in graph.astream_events(
        {"messages": [HumanMessage("xin chào")], "next_agent": ""},
        version="v2",
    ):
        tags = event.get("tags") or []
        if ROUTER_TAG in tags:
            router_events.append(event)
        elif event.get("event") == "on_chat_model_stream":
            answer_events.append(event)

    assert router_events, "phai co event mang tag supervisor_router de chat.py loai bo"
    assert answer_events, "phai van con event stream cua cau tra loi"


@pytest.mark.asyncio
async def test_supervisor_end_to_end_routes_to_forecast_agent(auth):
    graph = create_supervisor_graph(
        "s1",
        auth,
        llm=make_fake_llm([AIMessage("### Dự báo: Tiffy — Chi nhánh 1")]),
        router_llm=_StubStructuredLLM(RouteDecision(next_agent=FORECAST_AGENT)),
    )
    result = await graph.ainvoke(
        {"messages": [HumanMessage("dự báo nhập hàng tiffi")], "next_agent": ""}
    )
    assert result["next_agent"] == FORECAST_AGENT
    assert "Dự báo" in result["messages"][-1].content
