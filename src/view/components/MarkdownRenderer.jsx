import ReactMarkdown from 'react-markdown';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';
import 'katex/dist/katex.min.css';

function MarkdownRenderer({ content, onDocumentLinkClick }) {
  // Processa i collegamenti documenti <col>nome<col>
  const processDocumentLinks = (text) => {
    const linkRegex = /<col>([^<]+)<col>/g;
    const parts = [];
    let lastIndex = 0;
    let match;

    while ((match = linkRegex.exec(text)) !== null) {
      // Aggiungi il testo prima del link
      if (match.index > lastIndex) {
        parts.push({
          type: 'text',
          content: text.slice(lastIndex, match.index)
        });
      }
      
      // Aggiungi il link
      parts.push({
        type: 'link',
        content: match[1]
      });
      
      lastIndex = match.index + match[0].length;
    }
    
    // Aggiungi il testo rimanente
    if (lastIndex < text.length) {
      parts.push({
        type: 'text',
        content: text.slice(lastIndex)
      });
    }
    
    return parts;
  };

  const components = {
    p: ({ children }) => {
      // Converti children in stringa per processare i link
      const text = String(children);
      const parts = processDocumentLinks(text);
      
      if (parts.length === 0) {
        return <p>{children}</p>;
      }
      
      return (
        <p>
          {parts.map((part, index) => {
            if (part.type === 'link') {
              return (
                <span
                  key={index}
                  className="doc-link"
                  onClick={() => onDocumentLinkClick && onDocumentLinkClick(part.content)}
                >
                  {part.content}
                </span>
              );
            }
            return <span key={index}>{part.content}</span>;
          })}
        </p>
      );
    }
  };

  return (
    <div className="markdown-content">
      <ReactMarkdown
        remarkPlugins={[remarkMath]}
        rehypePlugins={[rehypeKatex]}
        components={components}
      >
        {content}
      </ReactMarkdown>
    </div>
  );
}

export default MarkdownRenderer;
