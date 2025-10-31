import { ReportResponse } from "@/api/reports/models";
import ReportCard from "@/components/ReportCard";
import { useUnit } from "effector-react";
import { $usersStore } from "@/store/reportsDashboard";

type Props = {
  title: string;
  reports: ReportResponse[];
};

const Section = ({ title, reports }: Props) => {
  const usersStore = useUnit($usersStore);

  return (
    <section className="flex flex-col gap-3">
      <h2 className="section-title text-base-content">{title}</h2>
      <div className="reports-summary-wrapper">
        {!!reports.length &&
          reports.map((report) => (
            <ReportCard
              key={report.id}
              report={report}
              usersStore={usersStore}
              className="border-error border-2"
            />
          ))}
      </div>
    </section>
  );
};

export default Section;
