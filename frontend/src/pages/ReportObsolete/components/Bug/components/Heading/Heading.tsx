type Props = {
  isNewBug: boolean;
  bugId?: number;
};

const Heading = ({ isNewBug, bugId }: Props) => {
  return (
    <span className="text-2xl">
      {!isNewBug ? (
        <>
          Баг <span className="text-gray-300">#{bugId}</span>
        </>
      ) : (
        "Новый баг"
      )}
    </span>
  );
};

export default Heading;
