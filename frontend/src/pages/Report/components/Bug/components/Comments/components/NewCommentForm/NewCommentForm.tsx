import { Send, Trash2 } from "lucide-react";
import AttachFileButton from "../../../AttachFileButton/AttachFileButton";
import { memo, useRef } from "react";
import Avatar from "@/components/Avatar/Avatar";

type NewCommentFormProps = {
  value: string;
  attachments: File[];
  isSubmitting: boolean;
  onChange: (v: string) => void;
  onSubmit: () => void;
  onPickFiles: (files: FileList | null) => void;
  onRemovePicked: (index: number) => void;
};

const NewCommentForm = memo((props: NewCommentFormProps) => {
  const {
    value,
    attachments,
    isSubmitting,
    onChange,
    onSubmit,
    onPickFiles,
    onRemovePicked,
  } = props;
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (value.trim() || attachments.length > 0) onSubmit();
    }
  };

  return (
    <div className="flex items-start gap-3">
      <Avatar width={8} />
      <div className="flex-1">
        <div className="flex gap-2 items-center">
          <textarea
            value={value}
            onChange={(e) => onChange(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Оставьте сообщение..."
            className="textarea textarea-bordered resize-none min-h-auto flex-1"
            rows={1}
            disabled={isSubmitting}
          />
          <div className="flex flex-row gap-1 items-center">
            <div className="p-2">
              <AttachFileButton
                onClick={() => fileInputRef.current?.click()}
                disabled={isSubmitting}
              />
            </div>
            <button
              className="btn btn-primary btn-sm p-2 btn-circle"
              onClick={onSubmit}
              disabled={
                isSubmitting || (!value.trim() && attachments.length === 0)
              }
              title="Отправить"
            >
              <Send className="w-4 h-4" />
            </button>
          </div>
        </div>
        {!!attachments.length && (
          <div className="flex flex-wrap gap-2 mt-2">
            {attachments.map((file, index) => (
              <div key={index} className="relative group">
                <span className="text-xs bg-base-200 px-2 py-1 rounded">
                  {file.name}
                </span>
                <button
                  className="absolute -top-1 -right-1 bg-error text-error-content rounded-full p-0.5 opacity-0 group-hover:opacity-100 transition-opacity duration-200 hover:bg-error-focus"
                  onClick={() => onRemovePicked(index)}
                  title="Убрать файл"
                >
                  <Trash2 className="w-3 h-3" />
                </button>
              </div>
            ))}
          </div>
        )}
        <input
          ref={fileInputRef}
          type="file"
          multiple
          style={{ display: "none" }}
          onChange={(e) => onPickFiles(e.target.files)}
        />
      </div>
    </div>
  );
});

export default NewCommentForm;
