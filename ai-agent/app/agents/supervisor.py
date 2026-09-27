import logging
from typing import Optional

from langchain_core.language_models.chat_models import BaseChatModel
from langgraph.graph import END, START, StateGraph

from app.agents.pharmacy_agent import create_pharmacy_agent
from app.agents.router import CHAT_AGENT, FORECAST_AGENT, build_router_node
from app.agents.state import AgentState
from app.models.chat import AuthContext

logger = logging.getLogger(__name__)


def _build_chat_node(auth: AuthContext, llm: Optional[BaseChatModel]):
    async def chat_node(state: AgentState) -> AgentState:
        agent = create_pharmacy_agent("", auth, llm=llm)
        result = await agent.ainvoke({"messages": list(state["messages"])})
        messages = result.get("messages", [])
        return {"messages": [messages[-1]] if messages else []}

    return chat_node


def _build_forecast_node(auth: AuthContext, llm: Optional[BaseChatModel]):
    async def forecast_node(state: AgentState) -> AgentState:
        from app.agents.forecast_agent import create_forecast_agent

        agent = create_forecast_agent(auth, llm=llm)
        result = await agent.ainvoke({"messages": list(state["messages"])})
        messages = result.get("messages", [])
        return {"messages": [messages[-1]] if messages else []}

    return forecast_node


def create_supervisor_graph(
    session_id: str,
    auth: AuthContext,
    llm: Optional[BaseChatModel] = None,
    router_llm: Optional[BaseChatModel] = None,
):
    """Lắp ráp và biên dịch graph điều phối. Cho phép inject LLM phục vụ test."""
    workflow = StateGraph(AgentState)

    workflow.add_node("router", build_router_node(router_llm or llm))
    workflow.add_node(CHAT_AGENT, _build_chat_node(auth, llm))
    workflow.add_node(FORECAST_AGENT, _build_forecast_node(auth, llm))

    workflow.add_edge(START, "router")
    workflow.add_conditional_edges(
        "router",
        lambda state: state.get("next_agent", CHAT_AGENT),
        {CHAT_AGENT: CHAT_AGENT, FORECAST_AGENT: FORECAST_AGENT},
    )
    workflow.add_edge(CHAT_AGENT, END)
    workflow.add_edge(FORECAST_AGENT, END)

    return workflow.compile()
