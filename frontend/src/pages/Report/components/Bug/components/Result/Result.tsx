import { ReactNode } from "react";
import BugTextarea from "./components/BugTextarea/BugTextarea";
import FilePreview from "./components/FilePreview/FilePreview";
import { Attachment } from "@/types/attachment";

type Props = {
  title: ReactNode;
  value: string;
  colorType: "success" | "error";
  onSave: (value: string) => void;
  onFileUpload: (file: File) => void;
  onDeleteAttachment: (attachmentId: number) => void;
  autoFocus?: boolean;
  attachments?: Attachment[];
  reportId?: number | null;
  bugId?: number;
  attachType?: number;
};

const Result = ({
  title,
  value,
  onSave,
  colorType,
  autoFocus = false,
  attachments = [],
  reportId,
  bugId,
  attachType,
  onFileUpload,
  onDeleteAttachment,
}: Props) => {
  return (
    <div
      className={`border-l-4 border-${colorType} pl-4 bg-${colorType}/5 rounded-r-lg p-3`}
    >
      <div className="flex items-center gap-2 mb-2">
        <span className="font-semibold">{title}</span>
      </div>
      <BugTextarea
        placeholder="Опишите ожидаемый результат..."
        value={value}
        onSave={onSave}
        rows={3}
        autoFocus={autoFocus}
      />
      {reportId && bugId && (
        <FilePreview
          attachments={attachments}
          reportId={reportId}
          bugId={bugId}
          attachType={attachType || 0}
          onFileUpload={onFileUpload}
          onDeleteAttachment={onDeleteAttachment}
        />
      )}
    </div>
  );
};

export default Result;
