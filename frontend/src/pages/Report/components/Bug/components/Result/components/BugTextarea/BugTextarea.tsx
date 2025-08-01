import { useState, useRef, useEffect, KeyboardEvent } from "react";

type Props = {
  value: string | null;
  placeholder?: string;
  onSave: (value: string) => void;
  rows?: number;
  autoFocus?: boolean;
};

const ResultTextarea = ({
  value,
  placeholder,
  onSave,
  rows = 3,
  autoFocus = false,
}: Props) => {
  const [editValue, setEditValue] = useState(value || "");
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    setEditValue(value || "");
  }, [value]);

  useEffect(() => {
    if (autoFocus && textareaRef.current) {
      textareaRef.current.focus();
      const len = textareaRef.current.value.length;
      textareaRef.current.setSelectionRange(len, len);
    }
  }, []);

  const handleSave = () => {
    const trimmedValue = editValue.trim();

    if (trimmedValue !== (value || "").trim()) {
      onSave(trimmedValue);
    }
  };

  const handleCancel = () => {
    setEditValue(value || "");
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      handleSave();
    } else if (event.key === "Escape") {
      event.preventDefault();
      handleCancel();
    }
  };

  const handleBlur = () => {
    handleSave();
  };

  return (
    <textarea
      ref={textareaRef}
      value={editValue}
      onChange={(e) => setEditValue(e.target.value)}
      onBlur={handleBlur}
      onKeyDown={handleKeyDown}
      className={`w-full textarea textarea-bordered text-sm w-full resize-none bg-base-100`}
      placeholder={placeholder}
      rows={rows}
    />
  );
};

export default ResultTextarea;
