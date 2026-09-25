import logging
from typing import Optional

from langchain_core.language_models.chat_models import BaseChatModel
from langgraph.prebuilt import create_react_agent

from app.models.chat import AuthContext
from app.services.llm_service import create_llm
from app.tools import create_all_tools
from app.agents.prompts import build_system_prompt

logger = logging.getLogger(__name__)


def build_langfuse_metadata(
    session_id: Optional[str], auth: Optional[AuthContext] = None
) -> Optional[dict]:
    """Metadata trace-level theo chuẩn langfuse v4.

    langfuse v4 handler đọc session_id / trace_name / user_id từ run metadata
    của root observation (keys 'langfuse_*'), không nhận qua constructor v2.
    """
    if not session_id:
        return None
    metadata = {
        "langfuse_session_id": session_id,
        "langfuse_trace_name": f"chat-{session_id}",
    }
    if auth and getattr(auth, "branch_id", None):
        metadata["langfuse_user_id"] = f"branch:{auth.branch_id}"
    return metadata


def build_agent_callbacks():
    """Tạo callback Langfuse cho request khi tracing đã được khởi tạo ở startup."""
    from app.services.langfuse_service import get_langfuse_client

    if get_langfuse_client() is None:
        return []

    try:
        from langfuse.langchain import CallbackHandler

        return [CallbackHandler()]
    except Exception as e:  # noqa: BLE001 - không để lỗi tracing làm chết chat
        logger.warning(f"Langfuse tracing disabled: {e}")
        return []


def create_pharmacy_agent(
    session_id: str,
    auth: AuthContext,
    llm: Optional[BaseChatModel] = None,
):
    """Tạo ReAct agent với khả năng inject LLM (phục vụ test)."""
    llm = llm or create_llm()
    tools = create_all_tools(auth)

    agent = create_react_agent(
        model=llm,
        tools=tools,
        prompt=build_system_prompt(),
    )

    return agent