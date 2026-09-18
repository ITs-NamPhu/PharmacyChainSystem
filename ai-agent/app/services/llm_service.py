from langchain_google_genai import ChatGoogleGenerativeAI

from app.config.settings import settings


def create_llm() -> ChatGoogleGenerativeAI:
    return ChatGoogleGenerativeAI(
        model=settings.LLM_MODEL,
        temperature=settings.LLM_TEMPERATURE,
        google_api_key=settings.GOOGLE_API_KEY,
    )
