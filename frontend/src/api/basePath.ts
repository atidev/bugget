export let extendedBasePath = "";

export function computeExtendedBasePath(pathname: string): string {
  if (!pathname) return "";
  let i = pathname.indexOf("/reports");
  if (i === -1) {
    i = pathname.indexOf("/search");
  }
  return i >= 0 ? pathname.slice(0, i) : "";
}

export function setExtendedBasePathFrom(pathname: string) {
  extendedBasePath = computeExtendedBasePath(pathname);
}
