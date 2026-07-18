"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { ArrowLeft, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { CourseForm } from "@/components/admin/course-form";
import { editCourse, getCourse } from "@/lib/services";
import type { Course } from "@/lib/types";

export default function EditCoursePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [course, setCourse] = useState<Course | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getCourse(id)
      .then((c) => {
        if (!c) setError("Course not found.");
        else setCourse(c);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Failed to load course"));
  }, [id]);

  return (
    <div className="space-y-5">
      <Link
        href="/admin/courses"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="h-4 w-4" /> Courses
      </Link>
      <h2 className="text-lg font-semibold">Edit course</h2>

      {error ? (
        <p className="text-sm text-destructive">{error}</p>
      ) : !course ? (
        <div className="grid min-h-[30vh] place-items-center">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
        </div>
      ) : (
        <CourseForm
          course={course}
          submitLabel="Save changes"
          onSubmit={async (payload) => {
            await editCourse(id, payload);
            toast.success("Course updated.");
            router.push("/admin/courses");
          }}
        />
      )}
    </div>
  );
}
