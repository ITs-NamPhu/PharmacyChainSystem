import logging
import threading
import time
from typing import Optional

from langfuse import Langfuse

from app.config.settings import settings

logger = logging.getLogger(__name__)

_client: Optional[Langfuse] = None
_lock = threading.Lock()
_INIT_MAX_ATTEMPTS = 3
_INIT_RETRY_DELAY_SECONDS = 2.0


def init_langfuse_client() -> Optional[Langfuse]:
    """Khởi tạo một Langfuse client duy nhất cho tiến trình."""
    global _client

    with _lock:
        if _client is not None:
            return _client

        if not settings.langfuse_enabled:
            logger.info("Langfuse tracing disabled: thieu LANGFUSE_PUBLIC_KEY/SECRET_KEY")
            return None

        for attempt in range(1, _INIT_MAX_ATTEMPTS + 1):
            client = None
            try:
                client = Langfuse(
                    public_key=settings.LANGFUSE_PUBLIC_KEY,
                    secret_key=settings.LANGFUSE_SECRET_KEY,
                    base_url=settings.LANGFUSE_BASE_URL,
                    environment=settings.LANGFUSE_TRACING_ENVIRONMENT,
                    release=settings.LANGFUSE_RELEASE,
                    sample_rate=settings.LANGFUSE_SAMPLE_RATE,
                )
                if not client.auth_check():
                    client.shutdown()
                    logger.warning("Langfuse authentication check failed")
                    return None

                _client = client
                logger.info(f"Langfuse tracing enabled: {settings.LANGFUSE_BASE_URL}")
                return _client
            except Exception as e:
                logger.warning(
                    f"Langfuse init attempt {attempt}/{_INIT_MAX_ATTEMPTS} failed: {e}"
                )
                if client is not None:
                    try:
                        client.shutdown()
                    except Exception:
                        pass
                if attempt < _INIT_MAX_ATTEMPTS:
                    time.sleep(_INIT_RETRY_DELAY_SECONDS * attempt)

        return None


def get_langfuse_client() -> Optional[Langfuse]:
    return _client


def flush_langfuse_client() -> None:
    if _client is None:
        return
    try:
        _client.flush()
    except Exception as e:
        logger.warning(f"Langfuse flush failed: {e}")


def shutdown_langfuse_client() -> None:
    global _client

    with _lock:
        client = _client
        _client = None

    if client is None:
        return

    try:
        client.flush()
        client.shutdown()
    except Exception as e:
        logger.warning(f"Langfuse shutdown failed: {e}")
