import { useEffect } from "react";
import { useUnit } from "effector-react";
import {
  dashboardPageOpened,
  recentlyResolvedSectionOpened,
  $recentlyResolvedReports,
} from "@/store/dashboard";
import { $responsibleReports, $participantReports } from "@/store/index";
import Section from "./components/Section";
import LastReportsSection from "./components/LastReportsSection";

const Home = () => {
  const responsibleReports = useUnit($responsibleReports);
  const participantReports = useUnit($participantReports);
  const recentlyResolvedReports = useUnit($recentlyResolvedReports);

  useEffect(() => {
    dashboardPageOpened();
  }, []);

  const handleRecentlyResolvedExpand = () => {
    recentlyResolvedSectionOpened();
  };

  return (
    <div className="flex flex-col gap-4">
      <Section
        title="Ответственный"
        reports={responsibleReports}
        className="border-error"
      />
      <Section title="Участник" reports={participantReports} />
      <LastReportsSection
        data={recentlyResolvedReports}
        onExpand={handleRecentlyResolvedExpand}
      />
    </div>
  );
};

export default Home;
