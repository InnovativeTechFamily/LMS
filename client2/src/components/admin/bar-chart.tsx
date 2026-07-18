import type { MonthData } from "@/lib/types";

/** Minimal dependency-free bar chart for the last-12-months analytics series. */
export function BarChart({ data }: { data: MonthData[] }) {
  const max = Math.max(1, ...data.map((d) => d.count));
  return (
    <div className="flex h-56 items-end gap-1.5">
      {data.map((d, i) => {
        const pct = (d.count / max) * 100;
        return (
          <div key={i} className="group flex flex-1 flex-col items-center gap-2">
            <div className="relative flex h-full w-full items-end">
              <div
                className="w-full rounded-t-md bg-brand-gradient transition-all group-hover:brightness-110"
                style={{ height: `${Math.max(pct, 2)}%` }}
              />
              <span className="pointer-events-none absolute -top-6 left-1/2 -translate-x-1/2 rounded bg-foreground px-1.5 py-0.5 text-[10px] font-medium text-background opacity-0 transition-opacity group-hover:opacity-100">
                {d.count}
              </span>
            </div>
            <span className="w-full truncate text-center text-[10px] text-muted-foreground">
              {d.month.split(" ")[0]}
            </span>
          </div>
        );
      })}
    </div>
  );
}
