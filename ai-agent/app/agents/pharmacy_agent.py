# from langchain.agents import create_agent, create_tool_calling_agent, AgentExecutor
# from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder

from langgraph.prebuilt import create_react_agent

from app.models.chat import AuthContext
from app.services.llm_service import create_llm
from app.tools import create_all_tools
from app.agents.prompts import SYSTEM_PROMPT
from app.agents.memory import memory_manager


def create_pharmacy_agent(session_id: str, auth: AuthContext):
    
    llm = create_llm()
    tools = create_all_tools(auth)

    # agent = create_agent(
    #     model=llm,
    #     tools=tools,
    #     prompt=SYSTEM_PROMPT,
    # )
    
    # dynamic_prompt = SYSTEM_PROMPT + f"""
    
    # THÔNG TIN NGƯỜI DÙNG HIỆN TẠI:
    # - User ID: {auth.user_id}
    # - Chi nhánh ID đang làm việc: {auth.branch_id if auth.branch_id else "Toàn hệ thống (Admin)"}
    
    # Lưu ý: Mặc định hãy tra cứu/thao tác dữ liệu trên Chi nhánh ID này trừ khi người dùng yêu cầu xem chi nhánh khác.
    # """
    
    
    # prompt_template = ChatPromptTemplate.from_messages([
    #     ("system", SYSTEM_PROMPT),
        
    #     # Nơi chứa lịch sử chat cũ (được kéo từ .NET lên)
    #     MessagesPlaceholder(variable_name="chat_history", optional=True),
        
    #     # Câu hỏi mới nhất của người dùng
    #     ("user", "{input}"),
        
    #     # BẮT BUỘC: Nơi AI lưu nháp các bước gọi Tool và kết quả trả về
    #     MessagesPlaceholder(variable_name="agent_scratchpad"),
    # ])
    
    # === plan diff
    # core_agent = create_tool_calling_agent(
    #     llm=llm,
    #     tools=tools,
    #     prompt=prompt_template,
    #     # state_modifier=dynamic_prompt
    # )

    # # 2. Bọc vào AgentExecutor để thực thi
    # agent_executor = AgentExecutor(
    #     agent=core_agent,
    #     tools=tools,
    #     verbose=True,
    #     handle_parsing_errors=True
    # )

    agent = create_react_agent(
        model=llm,
        tools=tools,
        prompt=SYSTEM_PROMPT
    )
    return agent
