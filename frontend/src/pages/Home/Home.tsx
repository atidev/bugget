import { useEffect } from "react";
import { useUnit } from "effector-react";
import { dashboardPageOpened } from "@/store/reportsDashboard";
import { $responsibleReports, $participantReports } from "@/store/index";
import Section from "./components/Section/Section";
import "./Home.css";

const Home = () => {
  const responsibleReports = useUnit($responsibleReports);
  const participantReports = useUnit($participantReports);

  useEffect(() => {
    dashboardPageOpened();
  }, []);

  return (
    <div className="text-base-content auto-rows-min gap-4">
      <Section title="Ответственный" reports={responsibleReports} />
      <div className="divider section-divider"></div>
      <Section title="Участник" reports={participantReports} />
    </div>
  );
};

export default Home;
