import formatTime from "./formatTime";
import formatRelative from "./formatRelative";

const getCommentTimeDisplay = (dateString: string) => {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return "только что";
  const now = new Date();
  const diffInMs = now.getTime() - date.getTime();
  const diffInDays = diffInMs / (1000 * 60 * 60 * 24);
  if (diffInDays < 1) {
    const diffInMinutes = diffInMs / (1000 * 60);
    if (diffInMinutes < 1) return "только что";
    return formatTime(dateString);
  }
  if (diffInDays < 2) return `вчера ${formatTime(dateString)}`;
  return formatRelative(dateString);
};

export default getCommentTimeDisplay;
