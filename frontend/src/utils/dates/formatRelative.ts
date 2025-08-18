const formatRelative = (dateString: string) => {
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return "только что";
  const now = new Date();
  const diffInMs = now.getTime() - date.getTime();
  const diffInMinutes = diffInMs / (1000 * 60);
  const diffInHours = diffInMinutes / 60;
  const diffInDays = diffInHours / 24;
  if (diffInMinutes < 1) return "только что";
  if (diffInDays < 1)
    return diffInHours < 1
      ? `${Math.floor(diffInMinutes)}м назад`
      : `${Math.floor(diffInHours)}ч назад`;
  if (diffInDays < 2) return "вчера";
  return `${Math.floor(diffInDays)}д назад`;
};

export default formatRelative;
