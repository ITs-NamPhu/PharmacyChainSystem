const formatTime = (ts) => {
    if (!ts) return '';
    const d = new Date(ts);
    if (isNaN(d.getTime())) return '';
    const today = new Date();
    const isToday =
        d.getDate() === today.getDate() &&
        d.getMonth() === today.getMonth() &&
        d.getFullYear() === today.getFullYear();
    const time = d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    return isToday ? time : `${d.toLocaleDateString('vi-VN')} ${time}`;
};

const ChatHistory = ({ conversations, loading, onSelect, onBack }) => {
    return (
        <div className="chat-history">
            <div className="chat-history__bar">
                <button className="chat-widget__btn chat-history__back" onClick={onBack} title="Quay lại chat">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                        <path d="M19 12H5M12 19l-7-7 7-7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                </button>
                <span className="chat-history__title">Lịch sử hội thoại</span>
            </div>

            <div className="chat-history__list">
                {loading && conversations.length === 0 && (
                    <div className="chat-history__empty">
                        <span className="spinner"></span>
                        <span>Đang tải...</span>
                    </div>
                )}

                {!loading && conversations.length === 0 && (
                    <div className="chat-history__empty">Chưa có hội thoại nào.</div>
                )}

                {conversations.map((conver) => (
                    <button
                        key={conver.id}
                        className="chat-history__item"
                        onClick={() => onSelect(conver.id)}
                    >
                        <div className="chat-history__item-title">{conver.title}</div>
                        {conver.preview && <div className="chat-history__item-preview">{conver.preview}</div>}
                        <div className="chat-history__item-time">{formatTime(conver.updatedAt)}</div>
                    </button>
                ))}
            </div>
        </div>
    );
};

export default ChatHistory;