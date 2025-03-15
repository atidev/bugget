import defaultAvaSrc from "./default-ava.png";

type Props = {
  src?: string;
  width?: number;
};

const Avatar = ({ src = defaultAvaSrc, width = 8 }: Props) => {
  return (
    <div className="avatar">
      <div className={`w-${width} rounded-full image-wrapper`}>
        <img
          src={src}
          alt="ava"
          style={{ width: `${2}em` }}
          className="avatar-img"
        />
      </div>
    </div>
  );
};

export default Avatar;
