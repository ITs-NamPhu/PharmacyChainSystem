from typing import Annotated, Sequence, TypedDict

from langchain_core.messages import BaseMessage
from langgraph.graph.message import add_messages


class AgentState(TypedDict):
    """State chung của graph Supervisor-Worker.

    `messages` là toàn bộ lịch sử hội thoại,
    `next_agent` là biến định tuyến router ghi để chọn node kế tiếp.
    """

    messages: Annotated[Sequence[BaseMessage], add_messages]
    next_agent: str
