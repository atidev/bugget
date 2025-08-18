import { Paperclip } from "lucide-react";

type Props = {
  disabled?: boolean;
  onClick: () => void;
};

const AttachFileButton = ({ disabled, onClick }: Props) => {
  return (
    <button
      className="rounded-full transition-colors duration-200 cursor-pointer"
      disabled={disabled}
      onClick={onClick}
      title="Прикрепить файл"
    >
      <Paperclip className="w-4 h-4 text-base-content/60 hover:text-base-content transition-colors duration-200" />
    </button>
  );
};

export default AttachFileButton;
