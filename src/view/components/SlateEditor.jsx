import React, { useMemo, useState, useCallback } from "react";
import { createEditor } from "slate";
import { Slate, Editable, withReact } from "slate-react";
import { withHistory } from "slate-history";

export default function SlateEditor() {
  const editor = useMemo(() => withHistory(withReact(createEditor())), []);
  const [value, setValue] = useState([
    {
      type: "paragraph",
      children: [{ text: "Start typing here..." }],
    },
  ]);

  const renderElement = useCallback((props) => <Element {...props} />, []);
  const renderLeaf = useCallback((props) => <Leaf {...props} />, []);

  return (
    <div className="p-4 border rounded-xl shadow-md">
      <Slate
        editor={editor}
        value={value}
        onChange={(newValue) => setValue(newValue)}
      >
        <Editable
          renderElement={renderElement}
          renderLeaf={renderLeaf}
          placeholder="Write something awesome..."
          spellCheck
          autoFocus
        />
      </Slate>
    </div>
  );
}

// Default element rendering
const Element = ({ attributes, children, element }) => {
  switch (element.type) {
    case "heading":
      return <h2 {...attributes}>{children}</h2>;
    default:
      return <p {...attributes}>{children}</p>;
  }
};

// Default text rendering
const Leaf = ({ attributes, children }) => {
  return <span {...attributes}>{children}</span>;
};
