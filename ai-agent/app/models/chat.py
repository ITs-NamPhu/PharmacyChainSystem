from pydantic import BaseModel, Field
from typing import Optional


class ChatRequest(BaseModel):
    message: str = Field(..., description="Tin nhắn từ user")
    session_id: str = Field(default="default", description="ID phiên chat")
    conversation_id: Optional[int] = Field(default=None, description="ID hội thoại trên .NET (null = hội thoại mới)")


class ChatResponse(BaseModel):
    response: str
    session_id: str
    tool_calls: list[dict] = []


class ToolCallLog(BaseModel):
    tool_name: str
    input: dict
    output: str
    success: bool = True
    error: Optional[str] = None


class AuthContext(BaseModel):
    token: str = Field(..., description="Authorization header value")
    branch_id: Optional[str] = Field(default=None, description="X-Branch-Id header value")

    model_config = {"arbitrary_types_allowed": True}
