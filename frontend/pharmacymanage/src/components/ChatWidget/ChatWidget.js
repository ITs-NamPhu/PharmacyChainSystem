import { useState, useRef, useEffect, useCallback, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import ChatMessage from './ChatMessage';
import ChatInput from './ChatInput';
import ChatHistory from './ChatHistory';
import { useChatStore, NEW_KEY } from '../../stores/chatStore';
import { sendMessage, clearChatHistory, parseSSEStream, isTokenExpiringSoon } from './chatService';
import { postRefreshToken } from '../../services/apiService';
import { doRefreshToken } from '../../redux/action/userAction';
import { store } from '../../redux/store';
import './ChatWidget.scss';

const ChatWidget = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [view, setView] = useState('chat'); // 'chat' | 'history'
    const [activeConversationId, setActiveConversationId] = useState(null);
    const [currentToolCall, setCurrentToolCall] = useState(null);
    const [isStreaming, setIsStreaming] = useState(false);

    const messagesRef = useRef(null);
    const messagesEndRef = useRef(null);
    const topSentinelRef = useRef(null);
    const stickToBottomRef = useRef(true);
    const prevScrollHeightRef = useRef(null);

    const token = useSelector((state) => state.user.account.access_token);
    const branchId = useSelector((state) => state.user.account.currentBranchId);
    const isAuthenticated = useSelector((state) => state.user.isAuthentication);
    const dispatch = useDispatch();

    const activeKey = activeConversationId ? String(activeConversationId) : NEW_KEY;

    // Store (Zustand)
    const messageCache = useChatStore((state) => state.messageCache);
    const fetchMessages = useChatStore((state) => state.fetchMessages);
    const appendMessage = useChatStore((state) => state.appendMessage);
    const updateStreamingAI = useChatStore((state) => state.updateStreamingAI);
    const appendError = useChatStore((state) => state.appendError);
    const finalizeConversation = useChatStore((state) => state.finalizeConversation);
    const clearConversation = useChatStore((state) => state.clearConversation);

    const messages = useMemo(
        () => messageCache[activeKey] || []
        , [messageCache, activeKey]
    );

    const isLoading = useChatStore((state) => !!state.isLoading[activeKey]);
    const hasMore = useChatStore((state) => !!state.hasMore[activeKey]);
    const conversations = useChatStore((state) => state.conversations);
    const loadingConversations = useChatStore((state) => state.loadingConversations);

    const scrollToBottom = useCallback((behavior = 'smooth') => {
        if (stickToBottomRef.current && messagesEndRef.current) {
            messagesEndRef.current.scrollIntoView({ behavior });
        }
    }, []);

    const handleScroll = useCallback(() => {
        const el = messagesRef.current;
        if (!el) return;
        const distanceFromBottom = el.scrollHeight - el.scrollTop - el.clientHeight;
        stickToBottomRef.current = distanceFromBottom < 60;
    }, []);

    // Tải tin nhắn khi mở hội thoại (cache-reuse nếu đã có)
    useEffect(() => {
        if (!isOpen) return;
        if (activeConversationId) {
            fetchMessages(activeConversationId, null);
        }
    }, [isOpen, activeConversationId, fetchMessages]);

    // Auto-scroll: chỉ cuộn xuống khi người dùng đang ở đáy (stickToBottom),
    // và bù vị trí khi prepend tin cũ thay vì nhảy xuống đáy.
    useEffect(() => {
        // Trường hợp vừa tải tin cũ (prepend) -> giữ nguyên vị trí đang đọc
        if (prevScrollHeightRef.current != null && messagesRef.current && view === 'chat') {
            const el = messagesRef.current;
            el.scrollTop += el.scrollHeight - prevScrollHeightRef.current;
            prevScrollHeightRef.current = null;
            return;
        }
        scrollToBottom();
    }, [messages, view, scrollToBottom]);

    // Reverse infinite scroll: khi cuộn lên chạm sentinel đầu danh sách -> tải tin cũ hơn
    const loadOlderMessages = useCallback(() => {
        const state = useChatStore.getState();
        if (!activeConversationId) return;
        const key = String(activeConversationId);
        const list = state.messageCache[key] || [];
        if (!state.hasMore[key] || state.isLoading[key] || list.length === 0) return;

        const el = messagesRef.current;
        if (el) prevScrollHeightRef.current = el.scrollHeight;
        fetchMessages(activeConversationId, list[0].id);
    }, [activeConversationId, fetchMessages]);

    useEffect(() => {
        const sentinel = topSentinelRef.current;
        const el = messagesRef.current;
        if (view !== 'chat' || !sentinel || !el || !hasMore) return;

        const observer = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting) {
                    loadOlderMessages();
                }
            },
            { root: el, threshold: 0.1 }
        );

        observer.observe(sentinel);
        return () => observer.disconnect();
    }, [view, hasMore, loadOlderMessages, messages.length]);

    // Gọi sang .NET lấy Access Token mới, lưu vào Redux; trả token mới (null nếu thất bại)
    const refreshTokenNow = async () => {
        const { access_token, refresh_token } = store.getState().user.account;
        const res = await postRefreshToken({ accessToken: access_token, refreshToken: refresh_token });
        if (res?.ec === 0) {
            dispatch(doRefreshToken(res));
            return res.dt.accessToken;
        }
        return null;
    };

    // Mở SSE đọc stream; trả true nếu gặp auth_error (đã chủ động đóng kết nối)
    const runStream = async (text, currentToken, key) => {
        const response = await sendMessage(text, currentToken, branchId, activeConversationId);
        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let aiContent = '';
        let authError = false;

        try {
            await parseSSEStream(reader, decoder, (data) => {
                if (data.type === 'text') {
                    aiContent += data.content;
                    updateStreamingAI(key, aiContent, false);
                } else if (data.type === 'tool_start') {
                    setCurrentToolCall(`Dang su dung: ${data.tool}...`);
                } else if (data.type === 'tool_end') {
                    setCurrentToolCall(null);
                } else if (data.type === 'done') {
                    updateStreamingAI(key, aiContent, true);
                    useChatStore.getState().fetchConversations(true);
                    if (data.conversationId && !activeConversationId) {
                        // Hội thoại mới: dời cache từ 'new' sang id thật
                        finalizeConversation(NEW_KEY, data.conversationId);
                        setActiveConversationId(data.conversationId);
                    }
                } else if (data.type === 'error') {
                    appendError(key, data.content || 'Da xay ra loi.');
                } else if (data.type === 'auth_error') {
                    // đóng kết nối SSE hiện tại, báo cho handleSend refresh + retry
                    authError = true;
                    reader.cancel();
                }
            });
        } catch (error) {
            // Mất kết nối mạng hoặc body đã bị đóng
        }
        return authError;
    };

    const handleSend = async (text) => {
        const getToken = () => store.getState().user.account.access_token;
        if (!getToken()) return;

        const key = activeConversationId ? String(activeConversationId) : NEW_KEY;

        // B1: Token sắp chết (< 1 phút)? -> âm thầm refresh trước khi gọi FastAPI
        if (isTokenExpiringSoon(getToken())) {
            const fresh = await refreshTokenNow();
            if (!fresh) {
                appendError(key, 'Phien dang nhap da het han. Vui long dang nhap lai.');
                return;
            }
        }
        if (!getToken()) return;

        const userMessage = {
            id: Date.now(),
            sender: 'user',
            content: text,
            timestamp: new Date().toISOString(),
            done: true,
        };

        stickToBottomRef.current = true;
        appendMessage(key, userMessage);
        setIsStreaming(true);
        setCurrentToolCall(null);

        try {
            // B2: Stream + tự retry đúng 1 lần khi gặp auth_error (self-healing)
            let currentToken = getToken();
            for (let retry = 0; retry < 2; retry++) {
                if (isTokenExpiringSoon(currentToken)) {
                    const fresh = await refreshTokenNow();
                    if (!fresh) break;
                    currentToken = fresh;
                }
                const authError = await runStream(text, currentToken, key);
                if (!authError) break;
                const fresh = await refreshTokenNow();
                if (!fresh) break;
                currentToken = fresh;
            }
        } catch (error) {
            appendError(key, 'Khong the ket noi toi AI Agent. Vui long thu lai.');
        } finally {
            setIsStreaming(false);
            setCurrentToolCall(null);
        }
    };

    const handleClear = async () => {
        if (token) {
            await clearChatHistory(token, activeConversationId ? String(activeConversationId) : 'default');
        }
        clearConversation(activeKey);
        setActiveConversationId(null);
        setView('chat');
        stickToBottomRef.current = true;
    };

    const openHistory = () => {
        useChatStore.getState().fetchConversations(true);
        setView('history');
    };

    const openConversation = (id) => {
        setActiveConversationId(id);
        setView('chat');
        stickToBottomRef.current = true;
        fetchMessages(id, null);
    };

    if (!isAuthenticated) return null;

    return (
        <div className="chat-widget">
            {isOpen && (
                <div className="chat-widget__panel">
                    <div className="chat-widget__header">
                        <div className="chat-widget__title">
                            <span className="chat-widget__icon">&#129302;</span>
                            <span>AI Nha Thuoc</span>
                        </div>
                        <div className="chat-widget__actions">
                            <button className="chat-widget__btn" onClick={openHistory} title="Lịch sử hội thoại">
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                                    <path d="M12 6v6l4 2M21 12a9 9 0 11-18 0 9 9 0 0118 0z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                </svg>
                            </button>
                            <button className="chat-widget__btn" onClick={handleClear} title="Xoa lich su">
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                                    <path d="M3 6h18M8 6V4a2 2 0 012-2h4a2 2 0 012 2v2m3 0v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6h14z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                </svg>
                            </button>
                            <button className="chat-widget__btn" onClick={() => setIsOpen(false)}>
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                                    <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                </svg>
                            </button>
                        </div>
                    </div>

                    {view === 'history' ? (
                        <ChatHistory
                            conversations={conversations}
                            loading={loadingConversations}
                            onSelect={openConversation}
                            onBack={() => setView('chat')}
                        />
                    ) : (
                        <>
                            <div
                                className="chat-widget__messages"
                                ref={messagesRef}
                                onScroll={handleScroll}
                            >
                                {hasMore && messages.length > 0 && (
                                    <div ref={topSentinelRef} className="chat-widget__load-older">
                                        {isLoading ? (
                                            <>
                                                <span className="spinner"></span>
                                                <span>Đang tải tin cũ...</span>
                                            </>
                                        ) : (
                                            <span>Cuộn lên để xem tin cũ</span>
                                        )}
                                    </div>
                                )}

                                {messages.length === 0 && !isLoading && view === 'chat' && (
                                    <div className="chat-widget__welcome">
                                        <div className="welcome-icon">&#129302;</div>
                                        <p>Xin chao! Toi la AI tro ly cua NHATHUOC OS.</p>
                                        <p>Toi co the giup ban:</p>
                                        <ul>
                                            <li>Tim kiem thong tin thuoc</li>
                                            <li>Kiem tra ton kho</li>
                                            <li>Xem doanh thu</li>
                                            <li>Canh bao thuoc sap het han</li>
                                        </ul>
                                        <p className="welcome-hint">Nhap cau hoi o duoi de bat dau!</p>
                                    </div>
                                )}

                                {messages.map((msg, idx) => (
                                    <ChatMessage key={msg.id ?? idx} message={msg} />
                                ))}

                                {currentToolCall && (
                                    <div className="chat-message chat-message--ai">
                                        <div className="chat-message__avatar"><span>AI</span></div>
                                        <div className="chat-message__content">
                                            <div className="chat-message__tool chat-message__tool--active">
                                                <span className="spinner"></span>
                                                <span>{currentToolCall}</span>
                                            </div>
                                        </div>
                                    </div>
                                )}

                                <div ref={messagesEndRef} />
                            </div>

                            <ChatInput onSend={handleSend} disabled={isStreaming} />
                        </>
                    )}
                </div>
            )}

            <button
                className={`chat-widget__bubble ${isOpen ? 'chat-widget__bubble--active' : ''}`}
                onClick={() => setIsOpen(!isOpen)}
            >
                {isOpen ? (
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                        <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                ) : (
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                        <path d="M21 11.5a8.38 8.38 0 01-.9 3.8 8.5 8.5 0 01-7.6 4.7 8.38 8.38 0 01-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 01-.9-3.8 8.5 8.5 0 014.7-7.6 8.38 8.38 0 013.8-.9h.5a8.48 8.48 0 018 8v.5z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                )}
            </button>
        </div>
    );
};

export default ChatWidget;