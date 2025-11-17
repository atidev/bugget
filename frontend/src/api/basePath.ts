export let extendedBasePath = "";

const TEAM_ROOT_RE = /\/workspaces\/[^/]+\/teams\/[^/]+(?=\/|$)/;

export function computeExtendedBasePath(pathname: string): string {
  console.log("computeExtendedBasePath", pathname);
  if (!pathname) return "";
  const clean = pathname.includes("://")
    ? (() => {
        try {
          return new URL(pathname).pathname;
        } catch {
          return pathname.split("#")[0].split("?")[0];
        }
      })()
    : pathname.split("#")[0].split("?")[0];

  const match = clean.match(TEAM_ROOT_RE);
  if (match) {
    const end = (match.index ?? 0) + match[0].length;
    return clean.slice(0, end);
  }

  return "";
}

export function setExtendedBasePathFrom(pathname: string) {
  extendedBasePath = computeExtendedBasePath(pathname);
}
