import { Attachment } from "@/types/attachement";
import { ChangeEvent, Ref, useRef, ClipboardEvent } from "react";
import ImageCarousel from "./components/ImageCarousel/ImageCarousel";

type Props = {
  title: string;
  value: string;
  onChange: (event: ChangeEvent<HTMLTextAreaElement>) => void;
  withAttachments: boolean;
  files: Attachment[];
  onFileChange: (event: ChangeEvent<HTMLInputElement> | ClipboardEvent<HTMLTextAreaElement>) => void;
  textareaRef: Ref<HTMLTextAreaElement>;
};

const Result = ({
  title,
  value,
  onChange,
  onFileChange,
  withAttachments,
  files,
  textareaRef,
}: Props) => {
  const fileInputRef = useRef<HTMLInputElement>(null);

  return (
    <div className="flex grow-1 flex-col mb-1">
      <span className="mt-1 mb-1 font-semibold text-xs">{title}</span>
      <textarea
        ref={textareaRef}
        value={value}
        onChange={onChange}
        onPaste={onFileChange}
        className="textarea w-full mb-3 p-3 bg-base-100 focus:outline-none"
      />
      {withAttachments && (
        <div className="attachments w-1/2 mt-auto">
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
