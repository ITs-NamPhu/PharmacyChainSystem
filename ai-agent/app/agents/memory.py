from collections import defaultdict
from typing import Optional

from langchain_core.chat_history import InMemoryChatMessageHistory

from app.config.settings import settings


class ChatMemoryManager:
    def __init__(self, max_messages: int = None):
        self.max_messages = max_messages or settings.MAX_CHAT_HISTORY
        self._store: dict[str, InMemoryChatMessageHistory] = defaultdict(
            InMemoryChatMessageHistory
        )

    def get_history(self, session_id: str) -> InMemoryChatMessageHistory:
        history = self._store[session_id]
        if len(history.messages) > self.max_messages:
            history.messages = history.messages[-self.max_messages:]
        return history

    def clear(self, session_id: str) -> None:
        if session_id in self._store:
            del self._store[session_id]

    def clear_all(self) -> None:
        self._store.clear()


memory_manager = ChatMemoryManager()
