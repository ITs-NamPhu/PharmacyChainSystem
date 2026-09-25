from typing import List, Optional

from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    # --- LLM Provider ---
    LLM_PROVIDER: str = "groq"  # groq | gemini
    GROQ_API_KEY: Optional[str] = None
    GROQ_LLM_MODEL: str = "openai/gpt-oss-20b"

    # Gemini / Google (dùng khi LLM_PROVIDER=gemini hoặc fallback)
    GOOGLE_API_KEY: Optional[str] = None
    GEMINI_API_KEY: Optional[str] = None
    GEMINI_LLM_MODEL: str = "gemini-3.6-flash"
    LLM_FALLBACK_MODEL: str = "gemini-3.6-flash"

    LLM_TEMPERATURE: float = 0.0
    MAX_CHAT_HISTORY: int = 20
    CORS_ORIGINS: List[str] = ["http://localhost:3000", "http://localhost:5000"]

    # --- Backend ---
    BACKEND_URL: str = "http://api:8080"

    # --- Langfuse tracing (tùy chọn: thiếu key thì tắt trace, không crash) ---
    LANGFUSE_PUBLIC_KEY: Optional[str] = None
    LANGFUSE_SECRET_KEY: Optional[str] = None
    LANGFUSE_BASE_URL: str = "http://localhost:3001"
    LANGFUSE_TRACING_ENVIRONMENT: Optional[str] = None
    LANGFUSE_RELEASE: Optional[str] = None
    LANGFUSE_SAMPLE_RATE: float = 1.0

    model_config = {"env_file": ".env", "extra": "ignore"}

    @property
    def effective_gemini_api_key(self) -> Optional[str]:
        return self.GEMINI_API_KEY or self.GOOGLE_API_KEY

    @property
    def langfuse_enabled(self) -> bool:
        return bool(self.LANGFUSE_PUBLIC_KEY and self.LANGFUSE_SECRET_KEY)


settings = Settings()