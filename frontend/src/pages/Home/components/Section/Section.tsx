import { ReportResponse } from "@/api/reports/models";
import ReportSummary from "./ReportSummary";

type Props = {
  title: string;
  reports: ReportResponse[];
};

const Section = ({ title, reports }: Props) => {
  return (
    <section className="flex flex-col gap-3">
      <h2 className="section-title text-base-content">{title}</h2>
      <div className="reports-summary-wrapper">
        {!!reports.length &&
          reports.map((report) => (
            <ReportSummary key={report.id} report={report} highlight />
          ))}
      </div>
    </section>
  );
};

export default Section;
