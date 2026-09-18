from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    GOOGLE_API_KEY: str = "AQ.Ab8RN6J6wITuAlUZemc59wVYid0HDPntERSw7U4G9jY6GNVing"
    BACKEND_URL: str = "http://api:8080"
    LLM_MODEL: str = "gemini-3.6-flash"
    LLM_TEMPERATURE: float = 1.0
    MAX_CHAT_HISTORY: int = 20
    CORS_ORIGINS: list[str] = ["http://localhost:3000", "http://localhost:5000"]

    model_config = {"env_file": ".env", "extra": "ignore"}


settings = Settings()
