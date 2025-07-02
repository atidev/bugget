import { ReactNode } from "react";

export type StatusOption<T> = {
  value: T;
  indicator: ReactNode;
};

type Props<T> = {
  status: T;
  options: StatusOption<T>[];
  onChange: (status: T) => void;
  className?: string;
};

const StatusSelect = <T,>({
  status,
  options,
  onChange,
  className = "",
}: Props<T>) => {
  const currentOption = options.find((option) => option.value === status);

  return (
    <div className={`dropdown dropdown-end ${className}`}>
      <label
        tabIndex={0}
        className="flex items-center gap-2 justify-start cursor-pointer hover:bg-base-200 p-2 rounded"
      >
        {currentOption?.indicator}
      </label>

      <ul
        tabIndex={0}
        className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-48 mt-1 z-10"
      >
        {options.map((option) => (
          <li key={String(option.value)} onClick={() => onChange(option.value)}>
            <a className="flex items-center gap-2">{option.indicator}</a>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default StatusSelect;
