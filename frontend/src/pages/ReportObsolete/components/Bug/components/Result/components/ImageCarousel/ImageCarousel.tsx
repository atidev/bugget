import { useState } from "react";

import "./ImageCarousel.css";
import { Attachment } from "@/typesObsolete/attachement";
import { API_URL } from "@/const";

type Props = {
  attachments: Attachment[];
};

function ImageCarousel({ attachments }: Props) {
  // Храним индекс выбранного изображения
  const [activeIndex, setActiveIndex] = useState<number | null>(null);

  // При клике на превью устанавливаем активный индекс
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

  return (
    <>
      <div className="attachments">
        {attachments.map((attachment, index) => {
          return (
            <img
              src={`${API_URL}v1/reports/${attachment.reportId}/bug/${attachment.bugId}/attachments/${attachment.id}/content`}
              key={attachment.id}
              alt="attachment"
              className="attachment-icon"
              onClick={() => handleImageClick(index)}
            />
          );
        })}
      </div>

      {/* Модалка с каруселью */}
      {activeIndex !== null && (
        <div className="modal modal-open" onClick={handleOverlayClick}>
          <div className="modal-box relative w-11/12 max-w-3xl">
            <img
              src={`${API_URL}v1/reports/${attachments[activeIndex].reportId}/bug/${attachments[activeIndex].bugId}/attachments/${attachments[activeIndex].id}/content`}
              alt="картинка бага"
              className="w-full h-full object-cover"
            />
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

export default ImageCarousel;
