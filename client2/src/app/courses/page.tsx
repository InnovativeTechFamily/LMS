"use client";

import { useEffect, useMemo, useState } from "react";
import { Search, SlidersHorizontal } from "lucide-react";
import { CourseCard, CourseCardSkeleton } from "@/components/course-card";
import { Input } from "@/components/ui/input";
import { getCourses, getLayout } from "@/lib/services";
import { cn } from "@/lib/utils";
import { LAYOUT_TYPES } from "@/lib/types";
import type { Course } from "@/lib/types";

type SortKey = "popular" | "rating" | "newest" | "priceLow";

const sorts: { key: SortKey; label: string }[] = [
  { key: "popular", label: "Most popular" },
  { key: "rating", label: "Top rated" },
  { key: "newest", label: "Newest" },
  { key: "priceLow", label: "Price: low to high" },
];

export default function CoursesPage() {
  const [courses, setCourses] = useState<Course[] | null>(null);
  const [layoutCategories, setLayoutCategories] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [query, setQuery] = useState("");
  const [category, setCategory] = useState<string>("All");
  const [sort, setSort] = useState<SortKey>("popular");

  useEffect(() => {
    getCourses().then(setCourses).catch((e) => setError(e.message));
    // Admin-managed categories drive the filter order; ignore failures (fall back to course-derived).
    getLayout(LAYOUT_TYPES.categories)
      .then((l) => setLayoutCategories(l?.categories?.map((c) => c.title) ?? []))
      .catch(() => {});
  }, []);

  // Count courses per category so each chip can show how many it holds.
  const counts = useMemo(() => {
    const map = new Map<string, number>();
    courses?.forEach((c) => {
      if (c.categories) map.set(c.categories, (map.get(c.categories) ?? 0) + 1);
    });
    return map;
  }, [courses]);

  // Canonical list = admin Categories layout (in order), then any course category not covered by it.
  const categories = useMemo(() => {
    const seen = new Set(layoutCategories);
    const extras = Array.from(counts.keys()).filter((c) => !seen.has(c));
    return ["All", ...layoutCategories, ...extras];
  }, [layoutCategories, counts]);

  const visible = useMemo(() => {
    if (!courses) return [];
    let list = courses.filter((c) => {
      const matchesQuery =
        !query ||
        c.name.toLowerCase().includes(query.toLowerCase()) ||
        c.tags?.toLowerCase().includes(query.toLowerCase());
      const matchesCat = category === "All" || c.categories === category;
      return matchesQuery && matchesCat;
    });
    list = [...list].sort((a, b) => {
      switch (sort) {
        case "rating":
          return (b.ratings ?? 0) - (a.ratings ?? 0);
        case "priceLow":
          return (a.price ?? 0) - (b.price ?? 0);
        case "newest":
          return (b.createdAt ?? "").localeCompare(a.createdAt ?? "");
        default:
          return (b.purchased ?? 0) - (a.purchased ?? 0);
      }
    });
    return list;
  }, [courses, query, category, sort]);

  return (
    <div className="container py-12">
      <header className="mb-8">
        <h1 className="text-4xl font-bold tracking-tight">All courses</h1>
        <p className="mt-2 text-muted-foreground">
          Explore our full catalog and find your next skill.
        </p>
      </header>

      {/* Controls */}
      <div className="mb-8 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div className="relative w-full max-w-md">
          <Search className="pointer-events-none absolute left-3.5 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search courses…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            className="pl-10"
          />
        </div>
        <div className="flex items-center gap-2 overflow-x-auto no-scrollbar">
          <SlidersHorizontal className="h-4 w-4 shrink-0 text-muted-foreground" />
          {sorts.map((s) => (
            <button
              key={s.key}
              onClick={() => setSort(s.key)}
              className={cn(
                "whitespace-nowrap rounded-full border px-3.5 py-1.5 text-sm transition-colors",
                sort === s.key
                  ? "border-primary bg-primary/10 text-primary"
                  : "border-border text-muted-foreground hover:text-foreground"
              )}
            >
              {s.label}
            </button>
          ))}
        </div>
      </div>

      {/* Category chips (admin-managed via the Categories layout) */}
      {categories.length > 1 && (
        <div className="mb-8 flex flex-wrap gap-2">
          {categories.map((c) => {
            const count = c === "All" ? courses?.length ?? 0 : counts.get(c) ?? 0;
            const active = category === c;
            return (
              <button
                key={c}
                onClick={() => setCategory(c)}
                className={cn(
                  "inline-flex items-center gap-2 rounded-full border px-4 py-1.5 text-sm transition-colors",
                  active
                    ? "border-transparent bg-brand-gradient text-white"
                    : "border-border text-muted-foreground hover:text-foreground"
                )}
              >
                {c}
                <span
                  className={cn(
                    "rounded-full px-1.5 text-xs tabular-nums",
                    active ? "bg-white/20 text-white" : "bg-secondary text-muted-foreground"
                  )}
                >
                  {count}
                </span>
              </button>
            );
          })}
        </div>
      )}

      {/* Grid */}
      {error ? (
        <div className="rounded-2xl border border-dashed border-border p-12 text-center text-muted-foreground">
          {error}
        </div>
      ) : (
        <>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {courses === null
              ? Array.from({ length: 6 }).map((_, i) => <CourseCardSkeleton key={i} />)
              : visible.map((c) => <CourseCard key={c._id} course={c} />)}
          </div>
          {courses !== null && visible.length === 0 && (
            <div className="rounded-2xl border border-dashed border-border p-12 text-center text-muted-foreground">
              No courses match your search.
            </div>
          )}
        </>
      )}
    </div>
  );
}
