import { Breadcrumb } from "@/types/ui";

type Props = {
  breadcrumbs: Breadcrumb[];
  className?: string;
};

const Breadcrumbs = ({ breadcrumbs, className }: Props) => (
  <div className={`breadcrumbs text-sm mx-4 ${className}`} >
    <ul>
  {breadcrumbs.map((breadcrumb) => {
    return (
      <li key={breadcrumb.path}>
        <a href={breadcrumb.path}>
          {breadcrumb.label}
        </a>
      </li>
    )
  })}
</ul>
  </div>
)

export default Breadcrumbs;