from fastapi import APIRouter
import httpx

router = APIRouter(tags=["Health"])


@router.get("/health")
async def health_check():
    return {"status": "healthy", "service": "pharmacy-ai-agent"}


@router.get("/health/backend")
async def health_check_backend():
    try:
        async with httpx.AsyncClient(timeout=5.0) as client:
            resp = await client.get("http://api:8080/health")
            return {"status": "healthy", "backend": resp.status_code}
    except Exception as e:
        return {"status": "unhealthy", "backend": str(e)}
