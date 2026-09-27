from typing import Optional

from langchain_core.language_models.chat_models import BaseChatModel
from langgraph.prebuilt import create_react_agent

from app.agents.forecast_prompts import build_forecast_system_prompt
from app.models.chat import AuthContext
from app.tools.forecast_tools import create_forecast_tools


def create_forecast_agent(auth: AuthContext, llm: Optional[BaseChatModel] = None):
    """Chuyên viên dự báo nhập hàng: ReAct agent chỉ dùng bộ tool dự báo."""
    from app.services.llm_service import create_llm

    return create_react_agent(
        model=llm or create_llm(),
        tools=create_forecast_tools(auth),
        prompt=build_forecast_system_prompt(),
    )
