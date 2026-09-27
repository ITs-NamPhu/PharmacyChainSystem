"""Trạm điều phối (router) của graph Supervisor.

Dùng LLM với `with_structured_output` để ép trả về đúng tên worker, giống mẫu
Supervisor-Worker chuẩn. Nếu LLM lỗi (structured output hỏng, hết token, timeout)
thì lùi về phân loại bằng từ khoá, và mặc định cuối cùng là `chat_agent` — an toàn
vì nhánh chat luôn xử lý được mọi câu hỏi trong phạm vi hệ thống.
"""

import logging
import unicodedata
from typing import Literal, Optional

from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.prompts import ChatPromptTemplate
from pydantic import BaseModel, Field

from app.agents.state import AgentState

logger = logging.getLogger(__name__)

CHAT_AGENT = "chat_agent"
FORECAST_AGENT = "forecast_agent"

# Tag để chat.py lọc event của router, tránh text định tuyến lẫn vào câu trả lời
ROUTER_TAG = "supervisor_router"

ROUTER_PROMPT = (
    "Bạn là trạm định tuyến của hệ thống nhà thuốc. Đọc câu hỏi cuối cùng của "
    "người dùng và chọn đúng chuyên viên xử lý.\n"
    "CHỌN 'forecast_agent' khi câu hỏi mang tính DỰ BÁO NHẬP HÀNG: dự báo, nên "
    "nhập gì, có cần nhập không, xu hướng bán, dịch bệnh, thời tiết ảnh hưởng, "
    "cảnh báo thiếu hụt có cần trữ thêm.\n"
    "CHỌN 'chat_agent' cho mọi câu hỏi tra cứu và thống kê thông thường: tồn kho, "
    "giá, hóa đơn, khách hàng, nhà cung cấp, doanh thu, top bán chạy."
)

# Từ khoá dự phòng khi LLM không dùng được
_FORECAST_HINTS = (
    "du bao", "du bao nhap hang", "nen nhap", "can nhap", "nhap them",
    "nhap gi", "nhap hang", "nhip hang", "ton kho thieu", "thieu hut",
    "bao gio het hang", "nen tru them", "xu huong ban", "co nen nhap",
)


class RouteDecision(BaseModel):
    next_agent: Literal["chat_agent", "forecast_agent"] = Field(
        description=(
            "Chọn 'forecast_agent' nếu người dùng yêu cầu dự báo, phân tích hoặc "
            "quyết định nhập hàng. Chọn 'chat_agent' cho câu hỏi tra cứu/thống kê thông thường."
        )
    )


def _normalize(text: str) -> str:
    """Bỏ dấu tiếng Việt + hạ chữ thường để so khớp từ khoá."""
    if not text:
        return ""
    decomposed = unicodedata.normalize("NFD", text)
    stripped = "".join(ch for ch in decomposed if not unicodedata.combining(ch))
    return stripped.replace("đ", "d").replace("Đ", "D").lower()


def heuristic_route(text: str) -> str:
    """Phân loại bằng từ khoá, chỉ dùng khi LLM không trả lời được."""
    normalized = _normalize(text)
    if any(hint in normalized for hint in _FORECAST_HINTS):
        return FORECAST_AGENT
    return CHAT_AGENT


def build_router_node(llm: Optional[BaseChatModel] = None):
    """Trả về node hàm định tuyến, cho phép inject LLM trong test."""
    from app.services.llm_service import create_llm

    prompt = ChatPromptTemplate.from_messages([
        ("system", ROUTER_PROMPT),
        ("human", "{input}"),
    ])

    async def router_node(state: AgentState) -> AgentState:
        messages = state.get("messages") or []
        last_user = ""
        for message in reversed(messages):
            role = getattr(message, "type", "")
            content = getattr(message, "content", "")
            if role == "human":
                last_user = content if isinstance(content, str) else str(content)
                break

        if not last_user:
            return {"next_agent": CHAT_AGENT}

        try:
            model = llm or create_llm()
            structured = model.with_structured_output(RouteDecision)
            chain = prompt | structured
            decision = await chain.ainvoke(
                {"input": last_user},
                config={"tags": [ROUTER_TAG]},
            )
            return {"next_agent": decision.next_agent}
        except Exception as e:  # noqa: BLE001 - router hỏng không được làm chết chat
            logger.warning(f"Router LLM that bai ({e}), chuyen sang phan loai tu khoa")
            return {"next_agent": heuristic_route(last_user)}

    return router_node
