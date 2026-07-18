"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  Check,
  Clock,
  PlayCircle,
  Users,
  BookOpen,
  Loader2,
  ArrowLeft,
} from "lucide-react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Ratings } from "@/components/ratings";
import { Skeleton } from "@/components/ui/skeleton";
import { getCourse, createOrder } from "@/lib/services";
import { useAuth } from "@/providers/auth-provider";
import { formatPrice, initials } from "@/lib/utils";
import type { Course } from "@/lib/types";

export default function CourseDetailPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const { user } = useAuth();
  const [course, setCourse] = useState<Course | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [enrolling, setEnrolling] = useState(false);

  useEffect(() => {
    getCourse(id)
      .then((c) => {
        if (!c) setError("Course not found.");
        else setCourse(c);
      })
      .catch((e) => setError(e.message));
  }, [id]);

  const owned = !!user?.courses?.some((c) => c.courseId === id);

  async function handleEnroll() {
    if (!user) {
      router.push(`/login?redirect=/course/${id}`);
      return;
    }
    if (owned) {
      toast.info("You're already enrolled in this course.");
      return;
    }
    setEnrolling(true);
    try {
      await createOrder(id);
      toast.success("Enrolled! You now have access to this course.");
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Could not enroll.");
    } finally {
      setEnrolling(false);
    }
  }

  if (error) {
    return (
      <div className="container flex flex-col items-center py-32 text-center">
        <p className="text-lg text-muted-foreground">{error}</p>
        <Button asChild variant="outline" className="mt-6">
          <Link href="/courses">
            <ArrowLeft className="h-4 w-4" /> Back to courses
          </Link>
        </Button>
      </div>
    );
  }

  if (!course) return <CourseDetailSkeleton />;

  const totalMinutes = course.courseData?.reduce((sum, c) => sum + (c.videoLength ?? 0), 0) ?? 0;

  return (
    <div className="relative">
      <div className="absolute inset-x-0 top-0 -z-10 h-80 bg-brand-radial" />
      <div className="container py-12">
        <div className="grid gap-10 lg:grid-cols-3">
          {/* Main */}
          <div className="lg:col-span-2">
            <div className="flex flex-wrap items-center gap-2">
              {course.categories && <Badge variant="accent">{course.categories}</Badge>}
              {course.level && <Badge variant="muted">{course.level}</Badge>}
            </div>
            <h1 className="mt-4 text-3xl font-bold tracking-tight md:text-4xl">{course.name}</h1>
            <p className="mt-4 max-w-2xl text-muted-foreground">{course.description}</p>

            <div className="mt-5 flex flex-wrap items-center gap-x-6 gap-y-2 text-sm">
              <span className="flex items-center gap-2">
                <Ratings value={course.ratings} />
                <span className="font-medium">{course.ratings?.toFixed(1) ?? "0.0"}</span>
                <span className="text-muted-foreground">({course.reviews?.length ?? 0})</span>
              </span>
              <span className="flex items-center gap-1.5 text-muted-foreground">
                <Users className="h-4 w-4" /> {course.purchased ?? 0} students
              </span>
              <span className="flex items-center gap-1.5 text-muted-foreground">
                <BookOpen className="h-4 w-4" /> {course.courseData?.length ?? 0} lectures
              </span>
            </div>

            {/* Benefits */}
            {course.benefits?.length > 0 && (
              <Section title="What you'll learn">
                <div className="grid gap-3 sm:grid-cols-2">
                  {course.benefits.map((b, i) => (
                    <div key={i} className="flex items-start gap-2.5">
                      <Check className="mt-0.5 h-5 w-5 shrink-0 text-primary" />
                      <span className="text-sm">{b.title}</span>
                    </div>
                  ))}
                </div>
              </Section>
            )}

            {/* Curriculum */}
            {course.courseData?.length > 0 && (
              <Section title="Course content">
                <div className="overflow-hidden rounded-2xl border border-border">
                  {course.courseData.map((lec, i) => (
                    <div
                      key={lec._id ?? i}
                      className="flex items-center justify-between gap-4 border-b border-border p-4 last:border-0"
                    >
                      <div className="flex min-w-0 items-center gap-3">
                        <PlayCircle className="h-5 w-5 shrink-0 text-primary" />
                        <div className="min-w-0">
                          <p className="truncate text-sm font-medium">{lec.title}</p>
                          {lec.videoSection && (
                            <p className="truncate text-xs text-muted-foreground">
                              {lec.videoSection}
                            </p>
                          )}
                        </div>
                      </div>
                      {lec.videoLength ? (
                        <span className="flex items-center gap-1 whitespace-nowrap text-xs text-muted-foreground">
                          <Clock className="h-3.5 w-3.5" /> {lec.videoLength} min
                        </span>
                      ) : null}
                    </div>
                  ))}
                </div>
              </Section>
            )}

            {/* Prerequisites */}
            {course.prerequisites?.length > 0 && (
              <Section title="Prerequisites">
                <ul className="space-y-2">
                  {course.prerequisites.map((p, i) => (
                    <li key={i} className="flex items-start gap-2.5 text-sm">
                      <span className="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-primary" />
                      {p.title}
                    </li>
                  ))}
                </ul>
              </Section>
            )}

            {/* Reviews */}
            {course.reviews?.length > 0 && (
              <Section title="Learner reviews">
                <div className="space-y-4">
                  {course.reviews.map((r) => (
                    <div key={r._id} className="rounded-2xl border border-border bg-card p-5">
                      <div className="flex items-center gap-3">
                        <span className="grid h-10 w-10 place-items-center rounded-full bg-brand-gradient text-sm font-semibold text-white">
                          {initials(r.user?.name)}
                        </span>
                        <div>
                          <p className="text-sm font-medium">{r.user?.name ?? "Learner"}</p>
                          <Ratings value={r.rating} />
                        </div>
                      </div>
                      <p className="mt-3 text-sm text-muted-foreground">{r.comment}</p>
                    </div>
                  ))}
                </div>
              </Section>
            )}
          </div>

          {/* Sidebar */}
          <aside className="lg:col-span-1">
            <div className="sticky top-24 overflow-hidden rounded-2xl border border-border bg-card shadow-xl">
              <div className="relative aspect-video">
                {course.thumbnail?.url ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img
                    src={course.thumbnail.url}
                    alt={course.name}
                    className="h-full w-full object-cover"
                  />
                ) : (
                  <div className="h-full w-full bg-brand-gradient" />
                )}
                <div className="absolute inset-0 grid place-items-center bg-black/30">
                  <PlayCircle className="h-14 w-14 text-white/90" />
                </div>
              </div>
              <div className="space-y-5 p-6">
                <div className="flex items-baseline gap-2">
                  <span className="text-3xl font-bold text-gradient">
                    {formatPrice(course.price)}
                  </span>
                  {course.estimatedPrice ? (
                    <span className="text-muted-foreground line-through">
                      {formatPrice(course.estimatedPrice)}
                    </span>
                  ) : null}
                </div>

                <Button className="w-full" size="lg" onClick={handleEnroll} disabled={enrolling}>
                  {enrolling ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : owned ? (
                    "Go to course"
                  ) : user ? (
                    "Enroll now"
                  ) : (
                    "Log in to enroll"
                  )}
                </Button>

                <ul className="space-y-2.5 text-sm text-muted-foreground">
                  <li className="flex items-center gap-2">
                    <BookOpen className="h-4 w-4" /> {course.courseData?.length ?? 0} lectures
                  </li>
                  <li className="flex items-center gap-2">
                    <Clock className="h-4 w-4" /> {totalMinutes} minutes total
                  </li>
                  <li className="flex items-center gap-2">
                    <Check className="h-4 w-4" /> Full lifetime access
                  </li>
                </ul>
              </div>
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <section className="mt-10">
      <h2 className="mb-4 text-xl font-semibold">{title}</h2>
      {children}
    </section>
  );
}

function CourseDetailSkeleton() {
  return (
    <div className="container py-12">
      <div className="grid gap-10 lg:grid-cols-3">
        <div className="space-y-4 lg:col-span-2">
          <Skeleton className="h-6 w-24" />
          <Skeleton className="h-10 w-3/4" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
        <Skeleton className="h-96 w-full rounded-2xl" />
      </div>
    </div>
  );
}
