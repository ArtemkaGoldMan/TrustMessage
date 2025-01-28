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
              code: ({node, ...props}) => (
                <code 
                  style={{
                    fontFamily: 'monospace',
                    backgroundColor: 'rgba(255, 255, 255, 0.1)',
                    padding: '2px 4px',
                    borderRadius: '3px'
                  }} 
                  {...props} 
                />
              ),
              a: ({node, ...props}) => <a style={{color: '#3498db'}} {...props} />
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
          <li><code>`code`</code> for <code>code</code></li>
          <li><code>```code block```</code> for code blocks</li>
          <li><code>[link](url)</code> for links</li>
        </ul>
      </div>
    </div>
  );
} 