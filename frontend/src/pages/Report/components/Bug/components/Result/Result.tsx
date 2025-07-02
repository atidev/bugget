import BugTextarea from "./components/BugTextarea/BugTextarea";
import { FileText, Paperclip, Trash2 } from "lucide-react";

type Props = {
  title: string;
  value: string;
  onSave: (value: string) => void;
  colorType: "success" | "error";
};

const Result = ({ title, value, onSave, colorType }: Props) => {
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
      />
      {/* Иконки прикреплений для ожидаемого результата */}
      <div className="flex gap-2 mt-2">
        <button className="btn btn-ghost btn-xs p-1">
          <Paperclip className="w-4 h-4 text-base-content/60" />
        </button>
        <button className="btn btn-ghost btn-xs p-1">
          <FileText className="w-4 h-4 text-info" />
        </button>
        <button className="btn btn-ghost btn-xs p-1">
          <Trash2 className="w-4 h-4 text-error" />
        </button>
      </div>
    </div>
  );
};

export default Result;
