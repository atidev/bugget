import { useCallback } from "react";

type UsePasteFileOptions = {
  onFileUpload: (file: File) => void | Promise<void>;
};

const usePasteFile = ({ onFileUpload }: UsePasteFileOptions) => {
  const handlePaste = useCallback(
    (event: React.ClipboardEvent<HTMLTextAreaElement>) => {
      const file = event.clipboardData?.items[0]?.getAsFile();
      if (!file) return;

      // Предотвращаем вставку текста при загрузке файла
      event.preventDefault();

      try {
        onFileUpload(file);
      } catch (err) {
        console.error("Ошибка при загрузке файла:", err);
      }
    },
    [onFileUpload]
  );

  return { handlePaste };
};

export default usePasteFile;
