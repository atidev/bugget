import { justNowString, yesterdayString, backInTimeString } from "@/const";
import { getDateDiffs } from "./dateDiffs";

const formatRelative = (date: Date) => {
  const { diffInMinutes, diffInHours, diffInDays } = getDateDiffs(date);

  if (diffInMinutes < 1) return justNowString;
  if (diffInDays < 1)
    return diffInHours < 1
      ? `${Math.floor(diffInMinutes)}м ${backInTimeString}`
      : `${Math.floor(diffInHours)}ч ${backInTimeString}`;
  if (diffInDays < 2) return yesterdayString;
  return `${Math.floor(diffInDays)}д ${backInTimeString}`;
};

export default formatRelative;
