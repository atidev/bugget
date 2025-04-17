import { Breadcrumb } from "@/types/ui";

type Props = {
  breadcrumbs: Breadcrumb[];
};

const Breadcrumbs = ({ breadcrumbs }: Props) => (
  <div className={`breadcrumbs text-sm mx-4 min-h-[52px] py-4`}>
    <ul>
      {breadcrumbs.map((breadcrumb) => {
        return (
          <li key={breadcrumb.path}>
            <a href={breadcrumb.path}>{breadcrumb.label}</a>
          </li>
        );
      })}
    </ul>
  </div>
);

export default Breadcrumbs;
