import React from "react";

interface SaveButtonProps {
  isChanged: boolean;
  onSave: () => void;
}

const SaveButton: React.FC<SaveButtonProps> = ({ isChanged, onSave }) => {
  return (
    <button
      onClick={onSave}
      className={`btn btn-info px-4 py-2 rounded text-white`}
      disabled={!isChanged}
    >
      Сохранить
    </button>
  );
};

export default SaveButton;
