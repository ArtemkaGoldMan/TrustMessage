import { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';

export default function MarkdownEditor({ value, onChange }) {
  const [preview, setPreview] = useState(false);

  return (
    <div className="markdown-editor">
      <div className="editor-tabs">
        <button
          type="button"
          className={!preview ? 'active' : ''}
          onClick={() => setPreview(false)}
        >
          Write
        </button>
        <button
          type="button"
          className={preview ? 'active' : ''}
          onClick={() => setPreview(true)}
        >
          Preview
        </button>
      </div>
      {preview ? (
        <div className="markdown-preview">
          <ReactMarkdown 
            remarkPlugins={[remarkGfm]}
            components={{
              p: ({node, ...props}) => <p style={{margin: '0 0 10px'}} {...props} />,
              strong: ({node, ...props}) => <strong style={{fontWeight: 'bold'}} {...props} />,
              em: ({node, ...props}) => <em style={{fontStyle: 'italic'}} {...props} />,
              a: ({node, ...props}) => <a style={{color: '#3498db'}} {...props} />,
              blockquote: ({node, ...props}) => (
                <blockquote style={{
                  borderLeft: '3px solid #ccc',
                  marginLeft: 0,
                  paddingLeft: '1rem',
                }} {...props} />
              ),
              ul: ({node, ...props}) => <ul style={{marginLeft: '1.5rem'}} {...props} />,
              ol: ({node, ...props}) => <ol style={{marginLeft: '1.5rem'}} {...props} />,
              li: ({node, ...props}) => <li style={{margin: '0.2rem 0'}} {...props} />
            }}
          >
            {value}
          </ReactMarkdown>
        </div>
      ) : (
        <textarea
          value={value}
          onChange={(e) => onChange(e.target.value)}
          placeholder="Write your message (Markdown supported)"
          required
          maxLength={1000}
        />
      )}
      <div className="markdown-help">
        <p>Markdown supported:</p>
        <ul>
          <li><code>**bold**</code> for <strong>bold</strong></li>
          <li><code>*italic*</code> for <em>italic</em></li>
          <li><code>[link](url)</code> for links</li>
          <li><code>{`>`} text</code> for blockquotes</li>
          <li><code>- item</code> for bullet lists</li>
          <li><code>1. item</code> for numbered lists</li>
          <li>Add blank line between text for new paragraph</li>
        </ul>
      </div>
    </div>
  );
} 