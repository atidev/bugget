import { useState, useRef } from "react";
import { FileText, Paperclip, Trash2 } from "lucide-react";
import { Attachment } from "@/types/attachment";
import { API_URL } from "@/const";

type Props = {
  attachments: Attachment[];
  reportId: number;
  bugId: number;
  attachType: number;
  onFileUpload?: (file: File) => void;
  onDeleteAttachment?: (attachmentId: number) => void;
};

const imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

const isImage = (fileName: string): boolean => {
  const extension = fileName.toLowerCase().substring(fileName.lastIndexOf("."));
  return imageExtensions.includes(extension);
};

function FilePreview({
  attachments,
  reportId,
  bugId,
  onFileUpload,
  onDeleteAttachment,
}: Props) {
  const [activeIndex, setActiveIndex] = useState<number | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleImageClick = (index: number) => {
    setActiveIndex(index);
  };

  const handleCloseModal = () => {
    setActiveIndex(null);
  };

  const handlePrev = () => {
    if (activeIndex !== null && activeIndex > 0) {
      setActiveIndex(activeIndex - 1);
    }
  };

  const handleNext = () => {
    if (activeIndex !== null && activeIndex < attachments.length - 1) {
      setActiveIndex(activeIndex + 1);
    }
  };

  const handleOverlayClick = (event: React.MouseEvent<HTMLDivElement>) => {
    if (event.target === event.currentTarget) {
      handleCloseModal();
    }
  };

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file && onFileUpload) {
      onFileUpload(file);
    }
    // Сбрасываем значение input для возможности загрузки того же файла
    event.target.value = "";
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleDeleteAttachment = (
    event: React.MouseEvent,
    attachmentId: number
  ) => {
    event.stopPropagation(); // Предотвращаем открытие карусели
    if (onDeleteAttachment) {
      onDeleteAttachment(attachmentId);
    }
  };

  // Получаем URL для превью
  const getPreviewUrl = (attachment: Attachment): string => {
    return `${API_URL}v2/reports/${reportId}/bugs/${bugId}/attachments/${attachment.id}/content`;
  };

  return (
    <>
      {/* Скрытый input для выбора файла */}
      <input
        ref={fileInputRef}
        type="file"
        style={{ display: "none" }}
        onChange={handleFileChange}
      />

      <div className="flex gap-2 mt-2">
        {attachments.map((attachment, index) => {
          const isImageFile = isImage(attachment.fileName);

          return (
            <div key={attachment.id} className="relative group">
              <button
                className="btn btn-ghost btn-xs p-1"
                onClick={() => handleImageClick(index)}
              >
                {isImageFile && attachment.hasPreview ? (
                  <img
                    src={getPreviewUrl(attachment)}
                    alt={attachment.fileName}
                    className="w-4 h-4 object-cover rounded"
                  />
                ) : (
                  <FileText className="w-4 h-4 text-info" />
                )}
              </button>

              <button
                className="absolute -top-1 -right-1 bg-error text-error-content rounded-full p-0.5 opacity-0 group-hover:opacity-100 transition-opacity duration-200 hover:bg-error-focus"
                onClick={(e) => handleDeleteAttachment(e, attachment.id)}
                title="Удалить файл"
              >
                <Trash2 className="w-3 h-3" />
              </button>
            </div>
          );
        })}
        <button
          className="btn btn-ghost btn-xs p-1"
          onClick={handleUploadClick}
        >
          <Paperclip className="w-4 h-4 text-base-content/60" />
        </button>
      </div>

      {/* Модалка с каруселью */}
      {activeIndex !== null && (
        <div className="modal modal-open" onClick={handleOverlayClick}>
          <div className="modal-box relative w-11/12 max-w-3xl">
            {isImage(attachments[activeIndex].fileName) ? (
              <img
                src={getPreviewUrl(attachments[activeIndex])}
                alt={attachments[activeIndex].fileName}
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="flex items-center justify-center h-64">
                <div className="text-center">
                  <FileText className="w-16 h-16 text-base-content/40 mx-auto mb-4" />
                  <p className="text-lg font-semibold">
                    {attachments[activeIndex].fileName}
                  </p>
                  <p className="text-sm text-base-content/60">Текстовый файл</p>
                </div>
              </div>
            )}

            {/* Стрелки навигации */}
            <div className="absolute left-5 right-5 top-1/2 flex justify-between transform -translate-y-1/2">
              {activeIndex > 0 && (
                <button
                  className="btn btn-circle absolute left-5"
                  onClick={handlePrev}
                >
                  ❮
                </button>
              )}

              {activeIndex < attachments.length - 1 && (
                <button
                  className="btn btn-circle absolute right-5"
                  onClick={handleNext}
                >
                  ❯
                </button>
              )}
            </div>
            <div className="modal-action">
              <button
                className="btn btn-soft btn-info"
                onClick={handleCloseModal}
              >
                Закрыть
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default FilePreview;
