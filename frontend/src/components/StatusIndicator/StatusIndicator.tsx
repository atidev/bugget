import { StatusMeta } from "@/types/status";
import { ReactElement } from "react";

export type StatusConfig<T extends string | number | symbol> = Record<
  T,
  {
    label: string;
    icon: React.ElementType;
    iconColor: string;
  }
>;

type Props = {
  statusMeta: StatusMeta;
};

const StatusIndicator = ({ statusMeta }: Props): ReactElement => {
  if (!statusMeta) {
    return (
      <div className="flex items-center space-x-2">
        <span className="text-sm text-base-content/50">Неизвестный статус</span>
      </div>
    );
  }

  const { title, icon: Icon, iconColor } = statusMeta;

  return (
    <div className="flex items-center space-x-2">
      <Icon className={`w-4 h-4 ${iconColor}`} />
      <span className="text-sm">{title}</span>
    </div>
  );
};

export default StatusIndicator;
