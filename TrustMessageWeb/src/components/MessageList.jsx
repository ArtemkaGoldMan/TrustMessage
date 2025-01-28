import React from 'react';

export default function MessageList({ messages }) {
  return (
    <div className="messages-container">
      {messages.length === 0 ? (
        <p>No messages to display</p>
      ) : (
        messages.map((message) => (
          <div key={message.id} className="message-card">
            <div className="message-header">
              <span className="username">{message.username}</span>
              <span className="date">{new Date(message.createdAt).toLocaleString()}</span>
            </div>
            <div className="message-content">
              <div dangerouslySetInnerHTML={{ __html: message.content }} />
            </div>
            <div className="message-footer">
              <span className={`verification ${message.isVerified ? 'verified' : 'not-verified'}`}>
                {message.isVerified ? 'Verified' : 'Not Verified'}
              </span>
            </div>
          </div>
        ))
      )}
    </div>
  );
} 