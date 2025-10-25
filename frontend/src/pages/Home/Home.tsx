import { useEffect } from "react";
import { useUnit } from "effector-react";
import {
  loadReportsFx,
  $responsibleReports,
  $participantReports,
} from "@/storeObsolete/reportsSummary";
import Section from "./components/Section/Section";
import "./Home.css";
import { $user } from "@/store/user";

const Home = () => {
  const responsibleReports = useUnit($responsibleReports);
  const participantReports = useUnit($participantReports);
  const user = useUnit($user);

  useEffect(() => {
    loadReportsFx(user.id);
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
