import { useState } from "react";
import defaultAvaSrc from "./default-ava.png";
import { RoundedSkeleton } from "../RoundedSkeleton/RoundedSkeleton";

type Props = {
  src?: string;
  width?: number;
};

const Avatar = ({ src = defaultAvaSrc, width = 8 }: Props) => {
  const [isLoaded, setIsLoaded] = useState(false);

  return (
    <div className="avatar block">
      {!isLoaded && (
        <div className="absolute inset-0">
          <RoundedSkeleton />
        </div>
      )}
      <div className={`w-${width} rounded-full image-wrapper`}>
        <img
          src={src}
          alt="ava"
          style={{ width: `${2}em` }}
          onLoad={() => setIsLoaded(true)}
        />
      </div>
    </div>
  );
};

export default Avatar;
