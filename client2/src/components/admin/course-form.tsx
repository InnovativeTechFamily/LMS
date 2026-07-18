"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { ImagePlus, Loader2, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent } from "@/components/ui/card";
import { Field } from "@/components/auth-shell";
import { readFileAsDataUrl } from "@/lib/utils";
import type { Course } from "@/lib/types";

interface LinkForm {
  title: string;
  url: string;
}
interface LectureForm {
  title: string;
  description: string;
  videoUrl: string;
  videoSection: string;
  videoLength: string;
  videoPlayer: string;
  links: LinkForm[];
  suggestion: string;
}
interface FormState {
  name: string;
  description: string;
  categories: string;
  price: string;
  estimatedPrice: string;
  tags: string;
  level: string;
  demoUrl: string;
  thumbnail: string;
  benefits: string[];
  prerequisites: string[];
  courseData: LectureForm[];
}

const emptyLecture: LectureForm = {
  title: "",
  description: "",
  videoUrl: "",
  videoSection: "Untitled Section",
  videoLength: "",
  videoPlayer: "",
  links: [],
  suggestion: "",
};

function fromCourse(c?: Course): FormState {
  return {
    name: c?.name ?? "",
    description: c?.description ?? "",
    categories: c?.categories ?? "",
    price: c?.price != null ? String(c.price) : "",
    estimatedPrice: c?.estimatedPrice != null ? String(c.estimatedPrice) : "",
    tags: c?.tags ?? "",
    level: c?.level ?? "",
    demoUrl: c?.demoUrl ?? "",
    thumbnail: c?.thumbnail?.url ?? "",
    benefits: c?.benefits?.length ? c.benefits.map((b) => b.title) : [""],
    prerequisites: c?.prerequisites?.length ? c.prerequisites.map((p) => p.title) : [""],
    courseData: c?.courseData?.length
      ? c.courseData.map((d) => ({
          title: d.title ?? "",
          description: d.description ?? "",
          videoUrl: d.videoUrl ?? "",
          videoSection: d.videoSection ?? "Untitled Section",
          videoLength: d.videoLength != null ? String(d.videoLength) : "",
          videoPlayer: d.videoPlayer ?? "",
          links: d.links?.map((l) => ({ title: l.title, url: l.url })) ?? [],
          suggestion: d.suggestion ?? "",
        }))
      : [{ ...emptyLecture }],
  };
}

function toPayload(f: FormState) {
  return {
    name: f.name,
    description: f.description,
    categories: f.categories,
    price: f.price ? Number(f.price) : 0,
    estimatedPrice: f.estimatedPrice ? Number(f.estimatedPrice) : undefined,
    tags: f.tags,
    level: f.level,
    demoUrl: f.demoUrl,
    thumbnail: f.thumbnail || undefined,
    benefits: f.benefits.filter((b) => b.trim()).map((title) => ({ title })),
    prerequisites: f.prerequisites.filter((p) => p.trim()).map((title) => ({ title })),
    courseData: f.courseData.map((d) => ({
      title: d.title,
      description: d.description,
      videoUrl: d.videoUrl,
      videoSection: d.videoSection,
      videoLength: d.videoLength ? Number(d.videoLength) : 0,
      videoPlayer: d.videoPlayer,
      suggestion: d.suggestion,
      links: d.links.filter((l) => l.title.trim() || l.url.trim()),
    })),
  };
}

export function CourseForm({
  course,
  onSubmit,
  submitLabel,
}: {
  course?: Course;
  onSubmit: (payload: ReturnType<typeof toPayload>) => Promise<void>;
  submitLabel: string;
}) {
  const router = useRouter();
  const [f, setF] = useState<FormState>(() => fromCourse(course));
  const [saving, setSaving] = useState(false);

  function set<K extends keyof FormState>(key: K, value: FormState[K]) {
    setF((prev) => ({ ...prev, [key]: value }));
  }

  async function onThumb(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      set("thumbnail", await readFileAsDataUrl(file));
    } catch {
      toast.error("Could not read image");
    }
  }

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    if (!f.name.trim()) {
      toast.error("Course name is required.");
      return;
    }
    setSaving(true);
    try {
      await onSubmit(toPayload(f));
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <form onSubmit={submit} className="space-y-6">
      {/* Basics */}
      <Card>
        <CardContent className="space-y-4">
          <h3 className="font-semibold">Course details</h3>
          <Field label="Name">
            <Input value={f.name} onChange={(e) => set("name", e.target.value)} required />
          </Field>
          <Field label="Description">
            <Textarea value={f.description} onChange={(e) => set("description", e.target.value)} />
          </Field>
          <div className="grid gap-4 sm:grid-cols-2">
            <Field label="Category">
              <Input value={f.categories} onChange={(e) => set("categories", e.target.value)} />
            </Field>
            <Field label="Level">
              <Input
                value={f.level}
                onChange={(e) => set("level", e.target.value)}
                placeholder="Beginner / Intermediate / Advanced"
              />
            </Field>
            <Field label="Price (₹)">
              <Input
                type="number"
                min="0"
                value={f.price}
                onChange={(e) => set("price", e.target.value)}
              />
            </Field>
            <Field label="Estimated price (₹)">
              <Input
                type="number"
                min="0"
                value={f.estimatedPrice}
                onChange={(e) => set("estimatedPrice", e.target.value)}
              />
            </Field>
            <Field label="Tags">
              <Input value={f.tags} onChange={(e) => set("tags", e.target.value)} />
            </Field>
            <Field label="Demo URL">
              <Input value={f.demoUrl} onChange={(e) => set("demoUrl", e.target.value)} />
            </Field>
          </div>

          <Field label="Thumbnail">
            <div className="flex items-center gap-4">
              <div className="h-20 w-32 shrink-0 overflow-hidden rounded-lg border border-border bg-secondary">
                {f.thumbnail ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img src={f.thumbnail} alt="" className="h-full w-full object-cover" />
                ) : (
                  <div className="grid h-full w-full place-items-center text-muted-foreground">
                    <ImagePlus className="h-5 w-5" />
                  </div>
                )}
              </div>
              <div className="flex-1 space-y-2">
                <Input
                  placeholder="Image URL"
                  value={f.thumbnail.startsWith("data:") ? "" : f.thumbnail}
                  onChange={(e) => set("thumbnail", e.target.value)}
                />
                <label className="inline-flex cursor-pointer items-center gap-2 text-sm text-primary hover:underline">
                  <ImagePlus className="h-4 w-4" /> Upload image
                  <input type="file" accept="image/*" hidden onChange={onThumb} />
                </label>
              </div>
            </div>
          </Field>
        </CardContent>
      </Card>

      {/* Benefits & prerequisites */}
      <div className="grid gap-6 md:grid-cols-2">
        <StringListCard
          title="What you'll learn"
          items={f.benefits}
          onChange={(items) => set("benefits", items)}
          placeholder="Benefit"
        />
        <StringListCard
          title="Prerequisites"
          items={f.prerequisites}
          onChange={(items) => set("prerequisites", items)}
          placeholder="Prerequisite"
        />
      </div>

      {/* Lectures */}
      <Card>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="font-semibold">Course content</h3>
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => set("courseData", [...f.courseData, { ...emptyLecture }])}
            >
              <Plus className="h-4 w-4" /> Add lecture
            </Button>
          </div>

          {f.courseData.map((lec, i) => (
            <div key={i} className="space-y-3 rounded-xl border border-border p-4">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Lecture {i + 1}</span>
                {f.courseData.length > 1 && (
                  <button
                    type="button"
                    onClick={() =>
                      set(
                        "courseData",
                        f.courseData.filter((_, idx) => idx !== i)
                      )
                    }
                    className="text-muted-foreground hover:text-destructive"
                    aria-label="Remove lecture"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                )}
              </div>
              <div className="grid gap-3 sm:grid-cols-2">
                <Input
                  placeholder="Lecture title"
                  value={lec.title}
                  onChange={(e) => updateLecture(f, set, i, { title: e.target.value })}
                />
                <Input
                  placeholder="Section"
                  value={lec.videoSection}
                  onChange={(e) => updateLecture(f, set, i, { videoSection: e.target.value })}
                />
                <Input
                  placeholder="Video URL / VdoCipher ID"
                  value={lec.videoUrl}
                  onChange={(e) => updateLecture(f, set, i, { videoUrl: e.target.value })}
                />
                <Input
                  type="number"
                  min="0"
                  placeholder="Length (min)"
                  value={lec.videoLength}
                  onChange={(e) => updateLecture(f, set, i, { videoLength: e.target.value })}
                />
              </div>
              <Textarea
                placeholder="Lecture description"
                value={lec.description}
                onChange={(e) => updateLecture(f, set, i, { description: e.target.value })}
                className="min-h-[70px]"
              />
            </div>
          ))}
        </CardContent>
      </Card>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="ghost" onClick={() => router.back()}>
          Cancel
        </Button>
        <Button type="submit" disabled={saving}>
          {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : submitLabel}
        </Button>
      </div>
    </form>
  );
}

function updateLecture(
  f: FormState,
  set: <K extends keyof FormState>(key: K, value: FormState[K]) => void,
  index: number,
  patch: Partial<LectureForm>
) {
  set(
    "courseData",
    f.courseData.map((l, i) => (i === index ? { ...l, ...patch } : l))
  );
}

function StringListCard({
  title,
  items,
  onChange,
  placeholder,
}: {
  title: string;
  items: string[];
  onChange: (items: string[]) => void;
  placeholder: string;
}) {
  return (
    <Card>
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between">
          <h3 className="font-semibold">{title}</h3>
          <Button
            type="button"
            variant="secondary"
            size="sm"
            onClick={() => onChange([...items, ""])}
          >
            <Plus className="h-4 w-4" />
          </Button>
        </div>
        {items.map((item, i) => (
          <div key={i} className="flex gap-2">
            <Input
              placeholder={placeholder}
              value={item}
              onChange={(e) => onChange(items.map((x, idx) => (idx === i ? e.target.value : x)))}
            />
            {items.length > 1 && (
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={() => onChange(items.filter((_, idx) => idx !== i))}
                className="shrink-0 text-muted-foreground hover:text-destructive"
                aria-label="Remove"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            )}
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
