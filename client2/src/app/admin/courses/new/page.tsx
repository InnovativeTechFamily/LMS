"use client";

import { useRouter } from "next/navigation";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { toast } from "sonner";
import { CourseForm } from "@/components/admin/course-form";
import { createCourse } from "@/lib/services";

export default function NewCoursePage() {
  const router = useRouter();

  return (
    <div className="space-y-5">
      <Link
        href="/admin/courses"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="h-4 w-4" /> Courses
      </Link>
      <h2 className="text-lg font-semibold">Create course</h2>
      <CourseForm
        submitLabel="Create course"
        onSubmit={async (payload) => {
          await createCourse(payload);
          toast.success("Course created.");
          router.push("/admin/courses");
        }}
      />
    </div>
  );
}
