import formatTime from "./formatTime";
import formatRelative from "./formatRelative";
import { justNowString, yesterdayString } from "@/const";
import { getDateDiffs } from "./dateDiffs";

const getCommentTimeDisplay = (dateString: string) => {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return justNowString;

  const { diffInMinutes, diffInDays } = getDateDiffs(date);

  if (diffInDays < 1) {
    if (diffInMinutes < 1) return justNowString;
    return formatTime(date);
  }
  if (diffInDays < 2) return `${yesterdayString} ${formatTime(date)}`;
  return formatRelative(date);
};

export default getCommentTimeDisplay;
