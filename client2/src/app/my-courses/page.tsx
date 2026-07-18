"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { GraduationCap, PlayCircle } from "lucide-react";
import { RequireAuth } from "@/components/guard";
import { Button } from "@/components/ui/button";
import { CourseCardSkeleton } from "@/components/course-card";
import { getCourses } from "@/lib/services";
import { useAuth } from "@/providers/auth-provider";
import type { Course } from "@/lib/types";

export default function MyCoursesPage() {
  return (
    <RequireAuth>
      <MyCoursesInner />
    </RequireAuth>
  );
}

function MyCoursesInner() {
  const { user } = useAuth();
  const [courses, setCourses] = useState<Course[] | null>(null);

  useEffect(() => {
    getCourses()
      .then((all) => {
        const owned = new Set(user?.courses?.map((c) => c.courseId) ?? []);
        setCourses(all.filter((c) => owned.has(c._id)));
      })
      .catch(() => setCourses([]));
  }, [user]);

  return (
    <div className="container py-12">
      <div className="mb-8 flex items-center gap-3">
        <div className="grid h-11 w-11 place-items-center rounded-xl bg-primary/15 text-primary">
          <GraduationCap className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">My Learning</h1>
          <p className="text-sm text-muted-foreground">Pick up where you left off.</p>
        </div>
      </div>

      {courses === null ? (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 3 }).map((_, i) => (
            <CourseCardSkeleton key={i} />
          ))}
        </div>
      ) : courses.length === 0 ? (
        <div className="grid place-items-center rounded-2xl border border-dashed border-border py-20 text-center">
          <div className="max-w-sm">
            <p className="text-lg font-medium">You haven&apos;t enrolled in any courses yet</p>
            <p className="mt-1 text-sm text-muted-foreground">
              Browse the catalog and start learning today.
            </p>
            <Button asChild className="mt-6">
              <Link href="/courses">Explore courses</Link>
            </Button>
          </div>
        </div>
      ) : (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {courses.map((course) => (
            <Link
              key={course._id}
              href={`/learn/${course._id}`}
              className="group flex flex-col overflow-hidden rounded-2xl border border-border bg-card transition-all hover:-translate-y-1 hover:border-primary/40 hover:shadow-2xl hover:shadow-primary/10"
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
                <div className="absolute inset-0 grid place-items-center bg-black/30 opacity-0 transition-opacity group-hover:opacity-100">
                  <PlayCircle className="h-12 w-12 text-white" />
                </div>
              </div>
              <div className="flex flex-1 flex-col gap-2 p-5">
                <h3 className="line-clamp-2 font-semibold leading-snug transition-colors group-hover:text-primary">
                  {course.name}
                </h3>
                <p className="text-sm text-muted-foreground">
                  {course.courseData?.length ?? 0} lectures
                </p>
                <span className="mt-auto inline-flex items-center gap-1.5 pt-2 text-sm font-medium text-primary">
                  Continue learning <PlayCircle className="h-4 w-4" />
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
