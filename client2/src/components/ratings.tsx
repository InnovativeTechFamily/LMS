import { Star } from "lucide-react";
import { cn } from "@/lib/utils";

export function Ratings({ value = 0, className }: { value?: number; className?: string }) {
  return (
    <div className={cn("flex items-center gap-0.5", className)} aria-label={`Rated ${value} of 5`}>
      {Array.from({ length: 5 }).map((_, i) => {
        const filled = i + 1 <= Math.round(value);
        return (
          <Star
            key={i}
            className={cn(
              "h-4 w-4",
              filled ? "fill-yellow-400 text-yellow-400" : "text-muted-foreground/40"
            )}
          />
        );
      })}
    </div>
  );
}
