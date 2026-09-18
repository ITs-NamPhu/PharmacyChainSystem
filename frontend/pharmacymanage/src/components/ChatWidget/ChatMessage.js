const ChatMessage = ({ message }) => {
    const { sender, content, toolCall, timestamp } = message;

    const formatTime = (ts) => {
        if (!ts) return '';
        return new Date(ts).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    };

    return (
        <div className={`chat-message ${sender === 'user' ? 'chat-message--user' : 'chat-message--ai'}`}>
            {sender === 'ai' && (
                <div className="chat-message__avatar">
                    <span>AI</span>
                </div>
            )}
            <div className="chat-message__content">
                {toolCall && (
                    <div className="chat-message__tool">
                        <span className="tool-icon">&#128269;</span>
                        <span className="tool-name">{toolCall}</span>
                    </div>
                )}
                {content && (
                    <div className="chat-message__text">
                        {content}
                    </div>
                )}
                <div className="chat-message__time">{formatTime(timestamp)}</div>
            </div>
        </div>
    );
};

export default ChatMessage;
