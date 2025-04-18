type Props = {
  isChanged: boolean;
  onSave: () => void;
  isLoading: boolean;
};

const SaveButton = ({ isChanged, onSave, isLoading }: Props) => {
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
