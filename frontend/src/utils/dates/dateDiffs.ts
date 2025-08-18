export const getDateDiffs = (date: Date, now: Date = new Date()) => {
  const diffInMs = now.getTime() - date.getTime();
  const diffInMinutes = diffInMs / (1000 * 60);
  const diffInHours = diffInMinutes / 60;
  const diffInDays = diffInHours / 24;

  return {
    diffInMs,
    diffInMinutes,
    diffInHours,
    diffInDays,
  };
};
