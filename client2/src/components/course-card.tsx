import Link from "next/link";
import { Users, PlayCircle } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Ratings } from "@/components/ratings";
import { cn, formatPrice } from "@/lib/utils";
import type { Course } from "@/lib/types";

export function CourseCard({ course }: { course: Course }) {
  return (
    <Link
      href={`/course/${course._id}`}
      className="group relative flex flex-col overflow-hidden rounded-2xl border border-border bg-card transition-all hover:-translate-y-1 hover:border-primary/40 hover:shadow-2xl hover:shadow-primary/10"
    >
      <div className="relative aspect-video overflow-hidden">
        {course.thumbnail?.url ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={course.thumbnail.url}
            alt={course.name}
            className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
          />
        ) : (
          <div className="h-full w-full bg-brand-gradient opacity-80" />
        )}
        <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 transition-opacity group-hover:opacity-100" />
        <div className="absolute left-3 top-3 flex gap-2">
          {course.level && <Badge variant="muted">{course.level}</Badge>}
          {course.categories && <Badge variant="accent">{course.categories}</Badge>}
        </div>
      </div>

      <div className="flex flex-1 flex-col gap-3 p-5">
        <h3 className="line-clamp-2 text-base font-semibold leading-snug transition-colors group-hover:text-primary">
          {course.name}
        </h3>

        <div className="flex items-center gap-2 text-sm">
          <Ratings value={course.ratings} />
          <span className="text-muted-foreground">{course.ratings?.toFixed(1) ?? "0.0"}</span>
        </div>

        <div className="mt-auto flex items-center justify-between pt-2 text-sm text-muted-foreground">
          <span className="flex items-center gap-1.5">
            <PlayCircle className="h-4 w-4" /> {course.courseData?.length ?? 0} lectures
          </span>
          <span className="flex items-center gap-1.5">
            <Users className="h-4 w-4" /> {course.purchased ?? 0}
          </span>
        </div>

        <div className="flex items-center gap-2 pt-1">
          <span className="text-lg font-bold text-gradient">{formatPrice(course.price)}</span>
          {course.estimatedPrice ? (
            <span className="text-sm text-muted-foreground line-through">
              {formatPrice(course.estimatedPrice)}
            </span>
          ) : null}
        </div>
      </div>
    </Link>
  );
}

export function CourseCardSkeleton() {
  return (
    <div className={cn("overflow-hidden rounded-2xl border border-border bg-card")}>
      <div className="aspect-video animate-pulse bg-muted" />
      <div className="space-y-3 p-5">
        <div className="h-4 w-3/4 animate-pulse rounded bg-muted" />
        <div className="h-3 w-1/2 animate-pulse rounded bg-muted" />
        <div className="h-6 w-1/3 animate-pulse rounded bg-muted" />
      </div>
    </div>
  );
}
