# from langchain_google_genai import ChatGoogleGenerativeAI

from app.config.settings import settings
from langchain_groq import ChatGroq

def create_llm() -> ChatGroq:
    return ChatGroq(
        model=settings.GROQ_LLM_MODEL,
        temperature=settings.LLM_TEMPERATURE,
        api_key=settings.GROQ_API_KEY,
    )
