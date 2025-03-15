import React from "react";

interface CancelButtonProps {
  isChanged: boolean;
  onReset: () => void;
}

const CancelButton: React.FC<CancelButtonProps> = ({ isChanged, onReset }) => {
  return (
    <button
      onClick={onReset}
      className={`px-4 py-2 btn btn-outline btn-info`}
      disabled={!isChanged}
    >
      Отменить
    </button>
  );
};

export default CancelButton;
