import {
  useState,
  useRef,
  useEffect,
  KeyboardEvent,
  forwardRef,
  useImperativeHandle,
} from "react";

type Props = {
  value: string;
  placeholder: string;
  autoFocus: boolean;
  rows?: number;
  onSave: (value: string) => void;
  onBlur: (value: string) => void;
  onInput: () => void;
  onPaste?: (event: React.ClipboardEvent<HTMLTextAreaElement>) => void;
};

const ResultTextarea = forwardRef<HTMLTextAreaElement, Props>(
  (
    {
      value,
      placeholder,
      autoFocus,
      rows = 1,
      onSave,
      onBlur,
      onInput,
      onPaste,
    },
    ref
  ) => {
    const [localValue, setLocalValue] = useState(value);
    const textareaRef = useRef<HTMLTextAreaElement>(null);
    // Позволяет родительскому компоненту получать прямой доступ к textarea элементу
    // через ref для синхронизации высоты полей ввода
    useImperativeHandle(ref, () => {
      if (!textareaRef.current) {
        throw new Error("Textarea ref is not available");
      }
      return textareaRef.current;
    });

    useEffect(() => {
      if (autoFocus && textareaRef.current) {
        textareaRef.current.focus();
        const len = textareaRef.current.value.length;
        textareaRef.current.setSelectionRange(len, len);
      }
    }, [autoFocus]);

    useEffect(() => {
      setLocalValue(value);
    }, [value]);

    const handleSave = () => {
      const trimmed = localValue.trim();
      if (trimmed !== (value || "").trim()) {
        onSave(trimmed);
      }
    };

    const handleBlur = () => {
      handleSave();
      onBlur(localValue);
    };

    const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
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
        onInput={onInput}
        onBlur={handleBlur}
        onKeyDown={handleKeyDown}
        onPaste={onPaste}
        placeholder={placeholder}
        rows={rows}
        className="w-full textarea textarea-bordered text-sm resize-none bg-base-100 overflow-y-hidden"
      />
    );
  }
);

export default ResultTextarea;
