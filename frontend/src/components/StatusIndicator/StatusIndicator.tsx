import { ReactElement } from "react";

export type StatusConfig<T extends string | number | symbol> = Record<
  T,
  {
    label: string;
    icon: React.ElementType;
    iconColor: string;
  }
>;

type Props<T extends string | number | symbol> = {
  status: T;
  config: StatusConfig<T>;
};

const StatusIndicator = <T extends string | number | symbol>({
  status,
  config,
}: Props<T>): ReactElement => {
  const statusInfo = config[status];

  if (!statusInfo) {
    return (
      <div className="flex items-center space-x-2">
        <span className="text-sm text-base-content/50">Неизвестный статус</span>
      </div>
    );
  }

  const { label, icon: Icon, iconColor } = statusInfo;

  return (
    <div className="flex items-center space-x-2">
      <Icon className={`w-4 h-4 ${iconColor}`} />
      <span className="text-sm">{label}</span>
    </div>
  );
};

export default StatusIndicator;
