import { useState, useRef, useEffect, KeyboardEvent } from "react";

type Props = {
  value: string | null;
  placeholder?: string;
  rows?: number;
  autoFocus?: boolean;
  onSave: (value: string) => void;
  onFocus?: () => void;
};

const ResultTextarea = ({
  value,
  onSave,
  placeholder,
  rows = 3,
  autoFocus = false,
}: Props) => {
  const [localValue, setEditValue] = useState(value || "");
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const focusedRef = useRef(false);

  useEffect(() => {
    if (!focusedRef.current) {
      setEditValue(value || "");
    }
  }, [value]);

  useEffect(() => {
    if (autoFocus && textareaRef.current) {
      textareaRef.current.focus();
      const len = textareaRef.current.value.length;
      textareaRef.current.setSelectionRange(len, len);
    }
  }, [autoFocus]);

  const handleSave = () => {
    const trimmed = localValue.trim();
    if (trimmed !== (value || "").trim()) {
      onSave(trimmed);
    }
  };

  const handleBlur = () => {
    focusedRef.current = false;
    // ОТЛОЖИМ сохранение на следующий тик:
    setTimeout(handleSave, 0);
  };

  const handleFocus = () => {
    focusedRef.current = true;
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSave();
    }
    if (e.key === "Escape") {
      e.preventDefault();
      setEditValue(value || "");
    }
  };

  return (
    <textarea
      ref={textareaRef}
      value={localValue}
      onChange={(e) => setEditValue(e.target.value)}
      onBlur={handleBlur}
      onFocus={handleFocus}
      onKeyDown={handleKeyDown}
      placeholder={placeholder}
      rows={rows}
      className="w-full textarea textarea-bordered text-sm resize-none bg-base-100"
    />
  );
};

export default ResultTextarea;
