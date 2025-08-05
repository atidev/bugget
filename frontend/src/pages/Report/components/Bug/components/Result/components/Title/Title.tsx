import { CircleSmall } from "lucide-react";

type Props = {
  text: string;
  color: string;
};

const Title = ({ text, color }: Props) => {
  return (
    <span className="inline-flex items-center">
      <CircleSmall size={20} color={color} fill={color} /> {text}
    </span>
  );
};

export default Title;
