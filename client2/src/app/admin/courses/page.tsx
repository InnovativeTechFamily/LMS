"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { toast } from "sonner";
import { Loader2, Pencil, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { Table, TD, TH, THead, TRow } from "@/components/ui/table";
import { deleteCourse, getAdminCourses } from "@/lib/services";
import { formatDate, formatPrice } from "@/lib/utils";
import type { Course } from "@/lib/types";

export default function AdminCoursesPage() {
  const [courses, setCourses] = useState<Course[] | null>(null);
  const [toDelete, setToDelete] = useState<Course | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    try {
      setCourses(await getAdminCourses());
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Failed to load courses");
      setCourses([]);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function confirmDelete() {
    if (!toDelete) return;
    setBusy(true);
    try {
      await deleteCourse(toDelete._id);
      setCourses((prev) => prev?.filter((c) => c._id !== toDelete._id) ?? null);
      toast.success("Course deleted.");
      setToDelete(null);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Could not delete course");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between gap-3">
        <div>
          <h2 className="text-lg font-semibold">Courses</h2>
          <p className="text-sm text-muted-foreground">{courses?.length ?? 0} total</p>
        </div>
        <Button asChild>
          <Link href="/admin/courses/new">
            <Plus className="h-4 w-4" /> New course
          </Link>
        </Button>
      </div>

      {courses === null ? (
        <div className="grid min-h-[30vh] place-items-center">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
        </div>
      ) : (
        <Table>
          <THead>
            <TRow>
              <TH>Course</TH>
              <TH>Price</TH>
              <TH>Rating</TH>
              <TH>Purchases</TH>
              <TH>Created</TH>
              <TH className="text-right">Actions</TH>
            </TRow>
          </THead>
          <tbody>
            {courses.map((c) => (
              <TRow key={c._id}>
                <TD>
                  <div className="flex items-center gap-3">
                    <div className="h-10 w-16 shrink-0 overflow-hidden rounded-md bg-secondary">
                      {c.thumbnail?.url ? (
                        // eslint-disable-next-line @next/next/no-img-element
                        <img src={c.thumbnail.url} alt="" className="h-full w-full object-cover" />
                      ) : (
                        <div className="h-full w-full bg-brand-gradient opacity-70" />
                      )}
                    </div>
                    <div className="min-w-0">
                      <p className="line-clamp-1 font-medium">{c.name}</p>
                      <p className="text-xs text-muted-foreground">{c.categories}</p>
                    </div>
                  </div>
                </TD>
                <TD>{formatPrice(c.price)}</TD>
                <TD className="text-muted-foreground">{c.ratings?.toFixed(1) ?? "0.0"}</TD>
                <TD className="text-muted-foreground">{c.purchased ?? 0}</TD>
                <TD className="text-muted-foreground">{formatDate(c.createdAt)}</TD>
                <TD className="text-right">
                  <div className="flex justify-end gap-1">
                    <Button asChild variant="ghost" size="icon" aria-label="Edit">
                      <Link href={`/admin/courses/${c._id}/edit`}>
                        <Pencil className="h-4 w-4" />
                      </Link>
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      aria-label="Delete"
                      onClick={() => setToDelete(c)}
                      className="text-muted-foreground hover:text-destructive"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TD>
              </TRow>
            ))}
            {courses.length === 0 && (
              <TRow>
                <TD colSpan={6} className="py-10 text-center text-muted-foreground">
                  No courses yet. Create your first one.
                </TD>
              </TRow>
            )}
          </tbody>
        </Table>
      )}

      <Modal open={!!toDelete} onClose={() => setToDelete(null)} title="Delete course">
        <p className="text-sm text-muted-foreground">
          Delete <span className="font-medium text-foreground">{toDelete?.name}</span>? This cannot be undone.
        </p>
        <div className="mt-6 flex justify-end gap-2">
          <Button variant="ghost" onClick={() => setToDelete(null)}>
            Cancel
          </Button>
          <Button variant="destructive" onClick={confirmDelete} disabled={busy}>
            {busy ? <Loader2 className="h-4 w-4 animate-spin" /> : "Delete"}
          </Button>
        </div>
      </Modal>
    </div>
  );
}
