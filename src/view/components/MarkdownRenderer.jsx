import ReactMarkdown from 'react-markdown';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';
import 'katex/dist/katex.min.css';

function MarkdownRenderer({ content, onDocumentLinkClick }) {
  // Preprocessa il contenuto per convertire i link personalizzati
  const processContent = (text) => {
    // Sostituisci <col>text<col> con [text](doc://text)
    return text.replace(/<col>([^<]+)<col>/g, '[🔗 $1](doc://$1)');
  };

  const components = {
    a: ({ node, href, children, ...props }) => {
      // Gestisci i link ai documenti
      if (href && href.startsWith('doc://')) {
        const docName = href.replace('doc://', '');
        return (
          <span
            className="doc-link"
            onClick={() => onDocumentLinkClick && onDocumentLinkClick(docName)}
            {...props}
          >
            {children}
          </span>
        );
      }
      return <a href={href} {...props}>{children}</a>;
    }
  };

  const processedContent = processContent(content);

  if (!processedContent.trim()) {
    return (
      <div className="markdown-content">
        <p style={{ color: 'var(--color-text-tertiary)', fontStyle: 'italic' }}>
          Start typing to see preview...
        </p>
      </div>
    );
  }

  return (
    <div className="markdown-content">
      <ReactMarkdown
        remarkPlugins={[remarkMath]}
        rehypePlugins={[rehypeKatex]}
        components={components}
      >
        {processedContent}
      </ReactMarkdown>
    </div>
  );
}

export default MarkdownRenderer;
