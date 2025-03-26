import { useState, useRef, useEffect } from "react";
import { ChevronDown, ArrowDownNarrowWide, ArrowUpWideNarrow } from "lucide-react";

export type SortOption<T = string> = {
  label: string;
  value: T;
};

type Props<T = string> = {
  options: SortOption<T>[];
  value: T;
  direction: "asc" | "desc";
  onChange: (value: T) => void;
  onToggleDirection: () => void;
  className?: string;
};

const SortDropdown = <T,>({
  options,
  value,
  direction,
  onChange,
  onToggleDirection,
  className = "",
}: Props<T>) => {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  const selectedLabel = options.find((opt) => opt.value === value)?.label;

  useEffect(() => {
    const handleOutsideClick = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, []);

  return (
    <div
      ref={ref}
      className={`inline-flex items-center rounded-md border border-gray-300 shadow-sm ${className}`}
    >
      <div className="relative">
        <button
          type="button"
          className="px-4 py-2 inline-flex gap-2 items-center text-sm font-medium text-base-content hover:bg-base-200"
          onClick={() => setOpen(!open)}
        >
          {selectedLabel}
          <ChevronDown className="h-4 w-4" />
        </button>

        {open && (
          <ul className="absolute z-50 mt-1 w-full bg-base-100 border border-gray-300 rounded-md shadow">
            {options.map((opt) => (
              <li key={String(opt.value)}>
                <button
                  type="button"
                  className={`w-full px-4 py-2 text-left text-sm hover:bg-base-200 ${opt.value === value ? "bg-base-200 font-semibold" : ""
                    }`}
                  onClick={() => {
                    onChange(opt.value);
                    setOpen(false);
                  }}
                >
                  {opt.label}
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>

      <button
        type="button"
        className="border-l border-gray-300 px-3 py-2 text-base-content hover:bg-base-200"
        onClick={onToggleDirection}
      >
        {direction === "asc" ? (
          <ArrowUpWideNarrow className="h-4 w-4" />
        ) : (
          <ArrowDownNarrowWide className="h-4 w-4" />
        )}
      </button>
    </div>
  );
};

export default SortDropdown;