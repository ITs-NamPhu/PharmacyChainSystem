"""LLM-as-judge: chấm điểm chất lượng câu trả lời bằng một LLM riêng (mặc định groq)."""
import json
import logging
from typing import Optional

from app.services.llm_service import create_groq_llm

logger = logging.getLogger(__name__)

_JUDGE_PROMPT = """Bạn là giám khảo chuyên nghiệp cho AI trợ lý ngành nhà thuốc/dược.
Đánh giá câu trả lời của agent dựa trên các yếu tố: đúng tool, số liệu có nguồn gốc từ tool (không bịa),
đầy đủ, ngôn ngữ tự nhiên dễ hiểu.

Câu hỏi người dùng: {question}
Danh mục: {category}
Tool dự kiến: {expected_tools}
Tool agent đã gọi: {called_tools}
Kết quả tool (context): {tool_outputs}
Câu trả lời của agent: {answer}

Chấm điểm 1-5 theo thang:
5 = Đúng, đầy đủ, số liệu chính xác từ tool, không thừa.
4 = Đúng nhưng thiếu một phần nhỏ thông tin.
3 = Dùng đúng tool nhưng còn sai sót số liệu hoặc thiếu chi tiết.
2 = Trả lời sai hướng hoặc bỏ qua tool cần thiết (hoặc gọi thừa tool).
1 = Trả lời sai hoàn toàn / bịa số liệu / không liên quan.

Chỉ trả về JSON chính xác dạng: {{"score": <1-5>, "reason": "<lý do ngắn gọn bằng tiếng Việt>"}}
"""


def judge_answer(question: str, category: str, expected_tools: list,
                 called_tools: list, answer: str, tool_outputs: list) -> dict:
    """Trả {score, reason}; nếu thiếu key groq hoặc lỗi -> score=None (skip)."""
    try:
        llm = create_groq_llm()
    except ValueError as e:
        logger.warning("judge skip (khong co GROQ_API_KEY): %s", e)
        return {"score": None, "reason": "no GROQ_API_KEY"}

    prompt = _JUDGE_PROMPT.format(
        question=question,
        category=category,
        expected_tools=", ".join(expected_tools) if expected_tools else "(không cần tool)",
        called_tools=", ".join(called_tools) if called_tools else "(không gọi tool)",
        tool_outputs=(" | ".join(tool_outputs)[:6000]) if tool_outputs else "(không có)",
        answer=(answer or "")[:2000],
    )
    try:
        raw = llm.invoke(prompt).content or ""
        return _parse(raw)
    except Exception as e:  # noqa: BLE001  - lỗi judge không được làm hỏng pipeline
        logger.warning("judge error: %s", e)
        return {"score": None, "reason": f"judge error: {e}"}


def _parse(raw: str) -> dict:
    text = raw.strip()
    try:
        data = json.loads(text[text.find("{"): text.rfind("}") + 1])
        return {"score": int(data.get("score")) if data.get("score") else None,
                "reason": str(data.get("reason", ""))}
    except (ValueError, TypeError):
        return {"score": None, "reason": f"unparseable judge output: {text[:200]}"}