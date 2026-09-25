import httpx
import pytest
import respx
import json

from app.api import chat as chat_module
from app.api.chat import (
    _normalize_token,
    create_conversation_in_dotnet,
    load_history_from_dotnet,
    save_history_to_dotnet,
)
from app.config.settings import settings
from langchain_core.messages import AIMessage, HumanMessage


def test_normalize_token_adds_bearer():
    assert _normalize_token("abc") == "Bearer abc"
    assert _normalize_token("Bearer abc") == "Bearer abc"
    assert _normalize_token("bearer abc") == "bearer abc"
    assert _normalize_token(None) is None


def test_normalize_token_does_not_duplicate():
    assert _normalize_token("Bearer abc") == "Bearer abc"


@pytest.mark.asyncio
async def test_load_history_maps_roles(auth):
    with respx.mock() as router:
        router.get(
            f"{settings.BACKEND_URL}/api/chat/conversations/{1}/messages"
        ).mock(
            return_value=httpx.Response(
                200,
                json={"dt": [
                    {"role": "user", "content": "hello"},
                    {"role": "AI", "content": "chao"},
                    {"role": "system", "content": "ignored"},
                ]},
            )
        )
        messages = await load_history_from_dotnet(1, auth)
    kinds = [type(m) for m in messages]
    assert HumanMessage in kinds
    assert AIMessage in kinds
    assert messages[0].content == "hello"
    assert messages[1].content == "chao"
    assert len(messages) == 2


@pytest.mark.asyncio
async def test_load_history_error_returns_empty(auth):
    with respx.mock() as router:
        router.get(
            f"{settings.BACKEND_URL}/api/chat/conversations/{2}/messages"
        ).mock(return_value=httpx.Response(500, text="boom"))
        messages = await load_history_from_dotnet(2, auth)
    assert messages == []


@pytest.mark.asyncio
async def test_create_conversation_returns_id(auth):
    with respx.mock() as router:
        router.post(f"{settings.BACKEND_URL}/api/chat/conversations").mock(
            return_value=httpx.Response(200, json={"dt": {"conversationId": 42}})
        )
        conv_id = await create_conversation_in_dotnet("hello", auth)
    assert conv_id == 42


@pytest.mark.asyncio
async def test_create_conversation_error_returns_none(auth):
    with respx.mock() as router:
        router.post(f"{settings.BACKEND_URL}/api/chat/conversations").mock(
            return_value=httpx.Response(500, text="boom")
        )
        conv_id = await create_conversation_in_dotnet("hello", auth)
    assert conv_id is None


@pytest.mark.asyncio
async def test_save_history_posts_payload_with_branch(auth):
    captured = {}

    def _capture(request):
        captured["json"] = json.loads(request.content.decode())
        captured["x_branch"] = request.headers.get("x-branch-id")
        return httpx.Response(200, json={"dt": None})

    with respx.mock() as router:
        router.post(f"{settings.BACKEND_URL}/api/chat/messages").mock(side_effect=_capture)
        await save_history_to_dotnet(7, "user msg", "ai msg", auth)
    assert captured["x_branch"] == "1"
    assert captured["json"] == {
        "conversationId": 7,
        "userMessage": "user msg",
        "aiMessage": "ai msg",
    }


@pytest.mark.asyncio
async def test_save_history_does_not_raise_on_error(auth):
    with respx.mock() as router:
        router.post(f"{settings.BACKEND_URL}/api/chat/messages").mock(
            return_value=httpx.Response(500, text="boom")
        )
        await save_history_to_dotnet(7, "u", "a", auth)  # không ném exception