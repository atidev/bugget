import { ReportResponse } from "@/api/reports/models";
import ReportCard from "@/components/ReportCard";
import { useUnit } from "effector-react";
import { $usersStore } from "@/store/dashboard";

type Props = {
  title: string;
  reports: ReportResponse[];
  className?: string;
};

const Section = ({ title, reports, className }: Props) => {
  const usersStore = useUnit($usersStore);

  return (
    <section className="flex flex-col gap-2">
      <div className="text-lg text-base-content">{title}</div>
      <div className="flex flex-col gap-1">
        {!!reports.length &&
          reports.map((report) => (
            <ReportCard
              key={report.id}
              report={report}
              usersStore={usersStore}
              className={className}
            />
          ))}
      </div>
    </section>
  );
};

export default Section;
