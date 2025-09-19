import { forwardRef } from "react";
import { Attachment } from "@/types/attachment";

import Title from "./components/Title/Title";
import FilePreview from "../FilePreview/FilePreview";
import ResultTextarea from "./components/ResultTextarea/ResultTextarea";

type Props = {
  reportId: number | null;
  bugId: number;
  title: string;
  value: string;
  colorType: "success" | "error";
  attachments: Attachment[];
  attachType: number;
  autoFocus: boolean;
  onSave: (value: string) => void;
  onBlur: (value: string) => void;
  onAttachmentUpload: (file: File) => void;
  onAttachmentDelete: (attachmentId: number) => void;
  onInput: () => void;
  onPaste?: (event: React.ClipboardEvent<HTMLTextAreaElement>) => void;
};

const Result = forwardRef<HTMLTextAreaElement, Props>(
  (
    {
      title,
      value,
      onSave,
      onBlur,
      colorType,
      autoFocus = false,
      attachments = [],
      reportId,
      bugId,
      attachType,
      onAttachmentUpload,
      onAttachmentDelete,
      onInput,
      onPaste,
    },
    ref
  ) => {
    return (
      <div
        className={`border-l-4 border-${colorType} pl-4 bg-${colorType}/5 rounded-r-lg p-3 flex flex-col`}
      >
        <div className="flex items-center gap-2 mb-2">
          <Title text={title} color={`var(--color-${colorType})`} />
        </div>
        <ResultTextarea
          ref={ref}
          placeholder={`Опишите ${title}...`}
          value={value || ""}
          onSave={onSave}
          onBlur={onBlur}
          autoFocus={autoFocus}
          onInput={onInput}
          onPaste={onPaste}
        />
        {reportId && bugId && (
          <FilePreview
            attachments={attachments}
            reportId={reportId}
            bugId={bugId}
            attachType={attachType || 0}
            onAttachmentUpload={onAttachmentUpload}
            onAttachmentDelete={onAttachmentDelete}
          />
        )}
      </div>
    );
  }
);

export default Result;
