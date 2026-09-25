import asyncio
import logging
from contextlib import asynccontextmanager

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.config.settings import settings
from app.api.chat import router as chat_router
from app.api.health import router as health_router
from app.services.langfuse_service import init_langfuse_client, shutdown_langfuse_client

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
)


@asynccontextmanager
async def lifespan(app: FastAPI):
    await asyncio.to_thread(init_langfuse_client)
    yield
    from app.services.backend_client import backend_client

    await backend_client.close()
    await asyncio.to_thread(shutdown_langfuse_client)


app = FastAPI(
    title="Pharmacy AI Agent",
    description="AI Agent cho he thong chuoi nha thuoc",
    version="1.0.0",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(health_router)
app.include_router(chat_router)
