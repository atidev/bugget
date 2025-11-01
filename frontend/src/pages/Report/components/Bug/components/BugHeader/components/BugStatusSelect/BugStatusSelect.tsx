import { BugStatuses, bugStatusMap } from "@/const";
import StatusIndicator from "@/components/StatusIndicator/StatusIndicator";
import StatusSelect from "@/components/StatusSelect/StatusSelect";

type Props = {
  status: BugStatuses;
  onChange: (status: BugStatuses) => void;
  className?: string;
};

const BugStatusSelect = ({ status, onChange, className }: Props) => {
  const bugStatusOptions = [
    {
      value: BugStatuses.ACTIVE,
      indicator: (
        <StatusIndicator statusMeta={bugStatusMap[BugStatuses.ACTIVE]} />
      ),
    },
    {
      value: BugStatuses.ARCHIVED,
      indicator: (
        <StatusIndicator statusMeta={bugStatusMap[BugStatuses.ARCHIVED]} />
      ),
    },
    {
      value: BugStatuses.REJECTED,
      indicator: (
        <StatusIndicator statusMeta={bugStatusMap[BugStatuses.REJECTED]} />
      ),
    },
  ];

  return (
    <StatusSelect
      status={status}
      options={bugStatusOptions}
      onChange={onChange}
      className={className}
    />
  );
};

export default BugStatusSelect;
