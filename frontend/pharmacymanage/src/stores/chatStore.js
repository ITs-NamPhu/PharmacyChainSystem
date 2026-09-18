import { create } from 'zustand';
import { getConversations, getMessages } from '../components/ChatWidget/chatService';

export const NEW_KEY = 'new';
const MESSAGE_LIMIT = 20;

// Zustand Store làm bộ nhớ đệm (Client Cache) cho tin nhắn chat.
// Cấu trúc: { [String(conversationId)]: Message[] } - mảng theo thứ tự cũ -> mới.
export const useChatStore = create((set, get) => ({
    messageCache: {},
    isLoading: {},
    hasMore: {},
    conversations: [],
    loadingConversations: false,

    // Tải tin nhắn, cache theo conversationId.
    // - Lần tải đầu (không có beforeId) + đã có cache -> bỏ qua gọi API.
    // - Có beforeId (cuộn lên) -> prepend tin cũ hơn lên đầu mảng.
    fetchMessages: async (conversationId, beforeId) => {
        const key = String(conversationId);
        const { isLoading, messageCache } = get();
        if (isLoading[key]) return;
        if (!beforeId && messageCache[key]) return;

        set((state) => ({ isLoading: { ...state.isLoading, [key]: true } }));
        try {
            const fetched = await getMessages(conversationId, beforeId);
            const existing = get().messageCache[key] || [];
            const merged = beforeId ? [...fetched, ...existing] : fetched;

            set((state) => ({
                messageCache: { ...state.messageCache, [key]: merged },
                hasMore: { ...state.hasMore, [key]: fetched.length >= MESSAGE_LIMIT },
                isLoading: { ...state.isLoading, [key]: false },
            }));
        } catch (error) {
            console.error('Lỗi tải tin nhắn:', error);
            set((state) => ({ isLoading: { ...state.isLoading, [key]: false } }));
        }
    },

    // Danh sách hội thoại cho màn hình Lịch sử
    fetchConversations: async (force = false) => {
        const { conversations, loadingConversations } = get();
        if (loadingConversations) return;
        if (!force && conversations.length > 0) return;

        set({ loadingConversations: true });
        try {
            const list = await getConversations();
            set({ conversations: list, loadingConversations: false });
        } catch (error) {
            console.error('Lỗi tải danh sách hội thoại:', error);
            set({ loadingConversations: false });
        }
    },

    // Thêm 1 tin nhắn mới vào cuối mảng (luồng gửi tin)
    appendMessage: (key, message) =>
        set((state) => ({
            messageCache: {
                ...state.messageCache,
                [key]: [...(state.messageCache[key] || []), message],
            },
        })),

    // Cập nhật tin AI đang stream: append-nếu-chưa-có / sửa-in-place nếu đang chạy
    updateStreamingAI: (key, content, done = false) =>
        set((state) => {
            const arr = [...(state.messageCache[key] || [])];
            const last = arr.length ? arr[arr.length - 1] : null;
            if (last && last.sender === 'ai' && !last.done) {
                arr[arr.length - 1] = { ...last, content, done };
            } else {
                arr.push({
                    id: Date.now(),
                    sender: 'ai',
                    content,
                    timestamp: new Date().toISOString(),
                    done,
                });
            }
            return { messageCache: { ...state.messageCache, [key]: arr } };
        }),

    // Thêm tin lỗi vào đáy
    appendError: (key, content) =>
        set((state) => {
            const arr = [
                ...(state.messageCache[key] || []),
                {
                    id: Date.now(),
                    sender: 'ai',
                    content,
                    timestamp: new Date().toISOString(),
                    done: true,
                },
            ];
            return { messageCache: { ...state.messageCache, [key]: arr } };
        }),

    // Hội thoại mới: dời mảng từ key tạm 'new' sang conversationId thật sau khi có id từ SSE
    finalizeConversation: (tempKey, conversationId) =>
        set((state) => {
            const messages = state.messageCache[tempKey] || [];
            const { [tempKey]: _drop, ...rest } = state.messageCache || {};
            return { messageCache: { ...rest, [String(conversationId)]: messages } };
        }),

    // Xóa cache của 1 key (chỉ cache phía client, KHÔNG xóa DB)
    clearConversation: (key) =>
        set((state) => {
            const { [key]: _drop, ...rest } = state.messageCache || {};
            return { messageCache: rest };
        }),
}));