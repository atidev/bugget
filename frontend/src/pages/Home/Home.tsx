import { useEffect } from "react";
import { useUnit } from "effector-react";
import {
  loadReportsFx,
  $responsibleReports,
  $participantReports,
} from "@/storeObsolete/reportsSummary";
import Section from "./components/Section/Section";
import "./Home.css";

const Main = () => {
  const responsibleReports = useUnit($responsibleReports);
  const participantReports = useUnit($participantReports);

  useEffect(() => {
    loadReportsFx();
  }, []);

  return (
    <div className="text-base-content auto-rows-min gap-4">
      <Section title="Ответственный" reports={responsibleReports} />
      <div className="divider section-divider"></div>
      <Section title="Участник" reports={participantReports} />
    </div>
  );
};

export default Main;
