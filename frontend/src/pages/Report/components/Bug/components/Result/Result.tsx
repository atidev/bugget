import { Attachment } from "@/types/attachement";
import { ChangeEvent, useEffect, useRef } from "react";
import ImageCarousel from "./components/ImageCarousel/ImageCarousel";

type Props = {
  title: string;
  value: string;
  onChange: (event: ChangeEvent<HTMLTextAreaElement>) => void;
  withAttachments: boolean;
  files: Attachment[];
  onFileChange: (event: ChangeEvent<HTMLInputElement>) => void;
};

const Result = ({
  title,
  value,
  onChange,
  onFileChange,
  withAttachments,
  files,
}: Props) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height = `${textareaRef.current.scrollHeight}px`;
    }
  }, [textareaRef]);

  return (
    <div className="flex grow-1 flex-col gap-3 text-xs font-semibold mb-1 mt-3">
      <span>{title}</span>
      <textarea
        ref={textareaRef}
        value={value}
        onChange={onChange}
        className="textarea w-full bug-section p-4 bg-base-100 focus:outline-none"
      />
      {withAttachments && (
        <div className="attachments w-1/2">
          {/* Скрытый input для выбора файла */}
          <input
            ref={fileInputRef}
            type="file"
            style={{ display: "none" }}
            onChange={onFileChange}
          />

          <button
            className="btn btn-info btn-outline mt-2"
            onClick={() => fileInputRef.current?.click()}
          >
            + Добавить файл
          </button>
          {files && !!files.length && <ImageCarousel attachments={files} />}
        </div>
      )}
    </div>
  );
};

export default Result;
