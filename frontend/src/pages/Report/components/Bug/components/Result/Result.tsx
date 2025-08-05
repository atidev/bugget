import { CircleSmall } from "lucide-react";

import { Attachment } from "@/types/attachment";

import FilePreview from "./components/FilePreview/FilePreview";
import BugTextarea from "./components/ResultTextarea/ResultTextarea";

type Props = {
  title: string;
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

const Title = ({ text, color }: { text: string; color: string }) => {
  return (
    <span className="inline-flex items-center">
      <CircleSmall size={20} color={color} fill={color} /> {text}
    </span>
  );
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
        <Title text={title} color={`var(--color-${colorType})`} />
      </div>
      <BugTextarea
        placeholder={`Опишите ${title}...`}
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
