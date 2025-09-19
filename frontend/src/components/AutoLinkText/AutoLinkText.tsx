import { useMemo, HTMLAttributes } from "react";

type Props = {
  text: string;
} & HTMLAttributes<HTMLDivElement>;

const AutoLinkText = ({ text, ...props }: Props) => {
  const linkifiedContent = useMemo(() => {
    if (!text) {
      return text;
    }
    const urlRegex = /(https?:\/\/[^\s]+|www\.[^\s]+)/g;
    const parts = text.split(urlRegex);

    return parts.map((part, index) => {
      if (part.match(urlRegex)) {
        const href = part.startsWith("http") ? part : `https://${part}`;
        return (
          <a
            key={index}
            href={href}
            target="_blank"
            rel="noopener noreferrer"
            className="text-blue-500 underline"
            onClick={(e) => e.stopPropagation()}
          >
            {part}
          </a>
        );
      }
      return part;
    });
  }, [text]);

  return <div {...props}>{linkifiedContent}</div>;
};

export default AutoLinkText;
