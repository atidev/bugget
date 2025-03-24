import { useState } from "react";
import { ChevronDown, X } from "lucide-react";

type DropdownOption<T = string> = {
    label: string;
    value: T;
};

type DropdownProps<T = string> = {
    label?: string;
    options: DropdownOption<T>[];
    value: T | T[] | null;
    onChange: (value: T | T[] | null) => void;
    multiple?: boolean;
    className?: string;
    defaultValue?: T | T[] | null;
};

const Dropdown = <T,>({
    label,
    options,
    value,
    onChange,
    multiple = false,
    className = "",
    defaultValue,
}: DropdownProps<T>) => {
    const [open, setOpen] = useState(false);

    const isSelected = (val: T) =>
        multiple
            ? Array.isArray(value) && value.includes(val)
            : val === value;

    const selected = multiple
        ? Array.isArray(value)
            ? value.length === 0
                ? ""
                : `${value.length} выбрано`
            : ""
        : value === null || value === undefined
            ? ""
            : options.find((opt) => opt.value === value)?.label ?? "";

    const hasValue = multiple
        ? Array.isArray(value) && value.length > 0
        : value !== null && value !== undefined;

    return (
        <div className={`dropdown min-w-[280px] ${className}`}>
            {label && <div className="mb-1 text-sm font-semibold">{label}</div>}
            <div
                tabIndex={0}
                className="btn w-full justify-between"
                onClick={() => setOpen(!open)}
            >
                <span className="flex gap-1 flex-wrap items-center">
                    {!hasValue ? (
                        <span className="text-gray-400 font-normal">Любой</span>
                    ) : multiple ? (
                        options
                            .filter((opt) => (Array.isArray(value) ? value.includes(opt.value) : false))
                            .map((opt) => (
                                <span
                                    key={String(opt.value)}
                                    className="bg-gray-200 rounded-full px-2 py-0.5 text-xs text-gray-700"
                                >
                                    {opt.label}
                                </span>
                            ))
                    ) : (
                        <span>{selected}</span>
                    )}
                </span>
                <span className="flex items-center">
                    {hasValue && (
                        <button
                            type="button"
                            className="inline-flex p-2"
                            onClick={(e) => {
                                e.stopPropagation();
                                onChange(defaultValue ?? (multiple ? [] : null));
                            }}
                        >
                            <X className="w-4 h-4 text-gray-400 hover:text-red-500" />
                        </button>
                    )}
                    <ChevronDown className="w-4 h-4 opacity-70" />
                </span>
            </div>

            <ul
                tabIndex={0}
                className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-full z-[1]"
            >
                {!multiple && (
                    <li key="none">
                        <a
                            onClick={() => {
                                onChange(null as T);
                                setOpen(false);
                            }}
                            className={value === null || value === undefined ? "active" : ""}
                        >
                            <div className="flex justify-between items-center text-gray-500 italic">
                                Не выбрано
                            </div>
                        </a>
                    </li>
                )}
                {options.map((opt) => (
                    <li key={String(opt.value)}>
                        <a
                            onClick={() => {
                                if (multiple) {
                                    const current = Array.isArray(value) ? value : [];
                                    const exists = current.includes(opt.value);
                                    const updated = exists
                                        ? current.filter((v) => v !== opt.value)
                                        : [...current, opt.value];
                                    onChange(updated);
                                } else {
                                    onChange(opt.value);
                                    setOpen(false);
                                }
                            }}
                            className={isSelected(opt.value) ? "active" : ""}
                        >
                            <div className="flex justify-between items-center">
                                {opt.label}
                                {isSelected(opt.value) && (
                                    <span className="text-success text-xs ml-2">✔</span>
                                )}
                            </div>
                        </a>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Dropdown;