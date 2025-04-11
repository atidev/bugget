type Props = {
  isNewBug: boolean;
  bugId?: number;
};

const Heading = ({ isNewBug, bugId }: Props) => {
  return isNewBug ? (
    <span className="text-2xl">Новый баг</span>
  ) : (
    <span className="text-2xl">
      Баг<span className="text-gray-300">#{bugId}</span>
    </span>
  );
};

export default Heading;
