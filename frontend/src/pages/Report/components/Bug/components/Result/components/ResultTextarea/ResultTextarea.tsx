import { useState, useRef, useEffect, KeyboardEvent } from "react";

type Props = {
  value: string;
  placeholder: string;
  autoFocus: boolean;
  rows: number;
  onSave: (value: string) => void;
  onBlur: (value: string) => void;
};

const ResultTextarea = ({
  value,
  placeholder,
  rows,
  autoFocus,
  onSave,
  onBlur,
}: Props) => {
  const [localValue, setLocalValue] = useState(value);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const focusedRef = useRef(false);

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
    onBlur(localValue);
  };

  const handleFocus = () => {
    focusedRef.current = true;
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      handleSave();
    }
    if (event.key === "Escape") {
      event.preventDefault();
      setLocalValue(value || "");
    }
  };

  return (
    <textarea
      ref={textareaRef}
      value={localValue}
      onChange={(event) => setLocalValue(event.target.value)}
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
