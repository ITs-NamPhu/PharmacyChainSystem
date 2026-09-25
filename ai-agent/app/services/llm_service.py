import logging
import unicodedata
from typing import Optional

from langchain_groq import ChatGroq

from app.config.settings import settings

logger = logging.getLogger(__name__)

# Provider hợp lệ dùng cho cấu hình/router
PROVIDERS = ("groq", "gemini")

# Gợi ý heuristic cho router (đã chuẩn hóa không dấu): câu thống kê/phân tích -> mô hình mạnh hơn
_COMPLEX_HINTS = [
    "tong", "doanh thu", "top", "nhieu nhat", "it nhat", "ban chay nhat",
    "cao nhat", "lon nhat", "diem", "bao nhieu", "thong ke", "so sanh",
    "trung binh", "chenh lech", "xu huong", "tang", "giam", "7 ngay",
    "30 ngay", "thang nay", "thang truoc", "tu ngay", "den ngay",
    "khoang thoi gian", "tuan",
]


def _strip_diacritics(text: str) -> str:
    """Bỏ dấu tiếng Việt -> khớp gợi ý kể cả khi người dùng gõ không dấu."""
    return "".join(
        c for c in unicodedata.normalize("NFD", text)
        if unicodedata.category(c) != "Mn"
    )


def _normalize(query: str) -> str:
    return _strip_diacritics(query).lower()


def create_groq_llm(model: Optional[str] = None) -> ChatGroq:
    if not settings.GROQ_API_KEY:
        raise ValueError("GROQ_API_KEY is required to create a Groq LLM")
    return ChatGroq(
        model=model or settings.GROQ_LLM_MODEL,
        temperature=settings.LLM_TEMPERATURE,
        api_key=settings.GROQ_API_KEY,
    )


def create_gemini_llm(model: Optional[str] = None):
    from langchain_google_genai import ChatGoogleGenerativeAI  # import chậm: lib nặng

    api_key = settings.effective_gemini_api_key
    if not api_key:
        raise ValueError("GEMINI_API_KEY / GOOGLE_API_KEY is required to create a Gemini LLM")
    return ChatGoogleGenerativeAI(
        model=model or settings.GEMINI_LLM_MODEL,
        temperature=settings.LLM_TEMPERATURE,
        google_api_key=api_key,
    )


def create_llm(provider: Optional[str] = None, model: Optional[str] = None):
    """Factory tạo chat model theo provider (groq | gemini)."""
    provider = (provider or settings.LLM_PROVIDER).lower()
    if provider not in PROVIDERS:
        logger.warning("Unknown LLM provider '%s', falling back to groq", provider)
        provider = "groq"
    if provider == "gemini":
        return create_gemini_llm(model)
    return create_groq_llm(model)


def route_query(query: str) -> str:
    """Chọn provider cho 1 câu hỏi dựa trên mức độ phức tạp.

    - Câu có gợi ý phân tích/so sánh -> gemini (mạnh hơn) nếu cấu hình cho phép.
    - Còn lại (tra cứu đơn giản) -> groq (nhanh, rẻ).
    """
    q = _normalize(query)
    if any(hint in q for hint in _COMPLEX_HINTS):
        if settings.effective_gemini_api_key:
            return "gemini"
        logger.debug("Complex query detected but Gemini key missing, using groq")
        return "groq"
    return "groq"


def create_llm_for_query(query: str, provider_override: Optional[str] = None):
    """Tạo LLM theo câu hỏi (router) với khả năng ghi đè qua header/env.

    - provider_override='auto' (hoặc mặc định) -> dùng route_query() để chọn provider.
    - provider_override cụ thể (groq|gemini) -> ưu tiên ghi đè của người gọi.
    """
    provider = (provider_override or "auto").lower()
    if provider == "auto":
        provider = route_query(query)
    return create_llm(provider)