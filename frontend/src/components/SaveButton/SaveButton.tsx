import React from "react";

interface SaveButtonProps {
  isChanged: boolean;
  onSave: () => void;
  isLoading: boolean;
}

const SaveButton: React.FC<SaveButtonProps> = ({
  isChanged,
  onSave,
  isLoading,
}) => {
  return (
    <button
      onClick={onSave}
      className={`btn btn-info px-4 py-2`}
      disabled={!isChanged}
    >
      {isLoading ? (
        <span className="loading loading-spinner"></span>
      ) : (
        "Сохранить"
      )}
    </button>
  );
};

export default SaveButton;
