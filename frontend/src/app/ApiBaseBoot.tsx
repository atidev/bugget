import { useEffect } from "react";
import { useLocation } from "react-router-dom";
import { setExtendedBasePathFrom } from "@/api/basePath";

export default function ApiBaseBoot() {
  const { pathname } = useLocation();
  useEffect(() => {
    setExtendedBasePathFrom(pathname);
  }, [pathname]);
  return null;
}
