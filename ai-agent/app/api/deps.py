from fastapi import Header, HTTPException, Request
from typing import Optional

from app.models.chat import AuthContext


async def get_auth_context(
    authorization: Optional[str] = Header(None),
    x_branch_id: Optional[str] = Header(None),
) -> AuthContext:
    if not authorization:
        raise HTTPException(
            status_code=401,
            detail="Missing Authorization header",
        )
    return AuthContext(
        token=authorization,
        branch_id=x_branch_id,
    )
