"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { ArrowRight } from "lucide-react";
import { CourseCard, CourseCardSkeleton } from "@/components/course-card";
import { Button } from "@/components/ui/button";
import { getCourses } from "@/lib/services";
import type { Course } from "@/lib/types";

export function FeaturedCourses() {
  const [courses, setCourses] = useState<Course[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getCourses()
      .then((all) =>
        setCourses([...all].sort((a, b) => (b.purchased ?? 0) - (a.purchased ?? 0)).slice(0, 6))
      )
      .catch((e) => setError(e.message));
  }, []);

  return (
    <section className="container py-20">
      <div className="mb-10 flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="mb-2 text-sm font-medium text-primary">Handpicked for you</p>
          <h2 className="text-3xl font-bold tracking-tight md:text-4xl">Featured courses</h2>
        </div>
        <Button asChild variant="outline">
          <Link href="/courses">
            Browse all <ArrowRight className="h-4 w-4" />
          </Link>
        </Button>
      </div>

      {error ? (
        <EmptyState message={error} />
      ) : (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {courses === null
            ? Array.from({ length: 6 }).map((_, i) => <CourseCardSkeleton key={i} />)
            : courses.length === 0
              ? <EmptyState message="No courses published yet." />
              : courses.map((c) => <CourseCard key={c._id} course={c} />)}
        </div>
      )}
    </section>
  );
}

function EmptyState({ message }: { message: string }) {
  return (
    <div className="col-span-full rounded-2xl border border-dashed border-border p-12 text-center text-muted-foreground">
      {message}
    </div>
  );
}
