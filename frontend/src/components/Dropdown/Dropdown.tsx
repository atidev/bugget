import { ChevronDown, X } from "lucide-react";

function getSingleSelectedLabel<T>(
    options: DropdownOption<T>[],
    value: T | null | undefined
): string {
    if (value === null || value === undefined) return "";
    return options.find((opt) => opt.value === value)?.label ?? "";
}

function getMultiSelectedLabel<T>(value: T[] | T | null | undefined): string {
    if (!Array.isArray(value)) return "";
    if (value.length === 0) return "";
    return `${value.length} выбрано`;
}

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
    onResetValue?: T | T[] | null;
};

const Dropdown = <T,>(props: DropdownProps<T>) => {
    const {
        label,
        options,
        value,
        onChange,
        multiple = false,
        className = "",
        onResetValue,
    } = props;

    const hasCustomResetValue = "onResetValue" in props;

    const isSelected = (val: T) =>
        multiple ? Array.isArray(value) && value.includes(val) : val === value;

    const selected = multiple
        ? getMultiSelectedLabel(value)
        : getSingleSelectedLabel(options, value as T);

    const hasValue = multiple
        ? Array.isArray(value) && value.length > 0
        : value !== null && value !== undefined;

    return (
        <div className={`dropdown ${className}`} tabIndex={0}>
            {label && <div className="mb-1 text-xs font-semibold">{label}</div>}
            <div className="btn bg-base-100 w-full justify-between">
                <span className="flex gap-1 flex-wrap items-center font-normal">
                    {!hasValue ? (
                        <span className="text-gray-400 font-normal">Любой</span>
                    ) : multiple ? (
                        options
                            .filter((opt) =>
                                Array.isArray(value) ? value.includes(opt.value) : false
                            )
                            .map((opt) => (
                                <span
                                    key={String(opt.value)}
                                    className="bg-base-300 rounded-full px-2 py-0.5 text-xs text-gray-700"
                                >
                                    {opt.label}
                                </span>
                            ))
                    ) : (
                        <span>{selected}</span>
                    )}
                </span>
                <span className="flex items-center">
                    {hasValue && hasCustomResetValue && (
                        <button
                            type="button"
                            className="inline-flex p-2"
                            onClick={(e) => {
                                e.stopPropagation();
                                onChange(onResetValue ?? (multiple ? [] : null));
                                (e.currentTarget.closest(".dropdown") as HTMLElement)?.blur();
                            }}
                        >
                            <X className="w-4 h-4 text-gray-400 hover:text-neutral cursor-pointer" />
                        </button>
                    )}
                    <ChevronDown className="w-4 h-4 opacity-70" />
                </span>
            </div>

            <ul className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-full z-[1]">
                {!multiple && hasCustomResetValue && (
                    <li key="none">
                        <button
                            onClick={(e) => {
                                onChange(null);
                                (e.currentTarget.closest(".dropdown") as HTMLElement)?.blur();
                            }}
                            className={value === null || value === undefined ? "active" : ""}
                        >
                            <div className="flex justify-between items-center text-gray-500 italic">
                                Не выбрано
                            </div>
                        </button>
                    </li>
                )}
                {options.map((opt) => (
                    <li key={String(opt.value)}>
                        <button
                            onClick={(e) => {
                                if (multiple) {
                                    const current = Array.isArray(value) ? value : [];
                                    const exists = current.includes(opt.value);
                                    const updated = exists
                                        ? current.filter((v) => v !== opt.value)
                                        : [...current, opt.value];
                                    onChange(updated);
                                } else {
                                    onChange(opt.value);
                                    (e.currentTarget.closest(".dropdown") as HTMLElement)?.blur();
                                }
                            }}
                            className={`hover:bg-base-200 ${isSelected(opt.value) ? "active bg-base-300" : ""}`}
                        >
                            <div className="flex justify-between items-center">
                                {opt.label}
                                {isSelected(opt.value) && (
                                    <span className="text-success text-xs ml-2">✔</span>
                                )}
                            </div>
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Dropdown;
