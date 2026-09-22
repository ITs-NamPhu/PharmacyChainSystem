from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    GROQ_API_KEY: str
    BACKEND_URL: str = "http://api:8080"
    GROQ_LLM_MODEL: str 
    LLM_TEMPERATURE: float = 0.0
    MAX_CHAT_HISTORY: int = 20
    CORS_ORIGINS: list[str] = ["http://localhost:3000", "http://localhost:5000"]

    model_config = {"env_file": ".env", "extra": "ignore"}


settings = Settings()
