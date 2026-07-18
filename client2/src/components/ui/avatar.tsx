import { cn, initials } from "@/lib/utils";

export function Avatar({
  name,
  url,
  className,
}: {
  name?: string;
  url?: string;
  className?: string;
}) {
  return (
    <span
      className={cn(
        "grid place-items-center overflow-hidden rounded-full bg-brand-gradient font-semibold text-white",
        className
      )}
    >
      {url ? (
        // eslint-disable-next-line @next/next/no-img-element
        <img src={url} alt={name ?? "avatar"} className="h-full w-full object-cover" />
      ) : (
        initials(name)
      )}
    </span>
  );
}
