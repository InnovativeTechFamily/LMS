"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { ImagePlus, Loader2, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent } from "@/components/ui/card";
import { Field } from "@/components/auth-shell";
import { getLayout, saveLayout } from "@/lib/services";
import { LAYOUT_TYPES } from "@/lib/types";
import { readFileAsDataUrl } from "@/lib/utils";
import type { FaqItem } from "@/lib/types";

export default function AdminContentPage() {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-semibold">Site content</h2>
        <p className="text-sm text-muted-foreground">
          Manage the homepage hero banner, FAQ and course categories.
        </p>
      </div>
      <BannerEditor />
      <FaqEditor />
      <CategoriesEditor />
    </div>
  );
}

/* ------------------------------ Banner ----------------------------- */

function BannerEditor() {
  const [title, setTitle] = useState("");
  const [subTitle, setSubTitle] = useState("");
  const [image, setImage] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.banner)
      .then((l) => {
        if (l?.banner) {
          setTitle(l.banner.title);
          setSubTitle(l.banner.subTitle);
          setImage(l.banner.image?.url ?? "");
        }
      })
      .finally(() => setLoading(false));
  }, []);

  async function onImage(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      setImage(await readFileAsDataUrl(file));
    } catch {
      toast.error("Could not read image");
    }
  }

  async function save() {
    setSaving(true);
    try {
      await saveLayout({ type: LAYOUT_TYPES.banner, title, subTitle, image });
      toast.success("Banner saved.");
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardContent className="space-y-4">
        <h3 className="font-semibold">Hero banner</h3>
        {loading ? (
          <Loader />
        ) : (
          <>
            <Field label="Title">
              <Input value={title} onChange={(e) => setTitle(e.target.value)} />
            </Field>
            <Field label="Subtitle">
              <Textarea value={subTitle} onChange={(e) => setSubTitle(e.target.value)} />
            </Field>
            <Field label="Image">
              <div className="flex items-center gap-4">
                <div className="h-20 w-32 shrink-0 overflow-hidden rounded-lg border border-border bg-secondary">
                  {image ? (
                    // eslint-disable-next-line @next/next/no-img-element
                    <img src={image} alt="" className="h-full w-full object-cover" />
                  ) : (
                    <div className="grid h-full w-full place-items-center text-muted-foreground">
                      <ImagePlus className="h-5 w-5" />
                    </div>
                  )}
                </div>
                <div className="flex-1 space-y-2">
                  <Input
                    placeholder="Image URL"
                    value={image.startsWith("data:") ? "" : image}
                    onChange={(e) => setImage(e.target.value)}
                  />
                  <label className="inline-flex cursor-pointer items-center gap-2 text-sm text-primary hover:underline">
                    <ImagePlus className="h-4 w-4" /> Upload image
                    <input type="file" accept="image/*" hidden onChange={onImage} />
                  </label>
                </div>
              </div>
            </Field>
            <Button onClick={save} disabled={saving}>
              {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : "Save banner"}
            </Button>
          </>
        )}
      </CardContent>
    </Card>
  );
}

/* ------------------------------- FAQ ------------------------------- */

function FaqEditor() {
  const [items, setItems] = useState<FaqItem[]>([{ question: "", answer: "" }]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.faq)
      .then((l) => {
        if (l?.faq?.length) setItems(l.faq);
      })
      .finally(() => setLoading(false));
  }, []);

  function update(i: number, patch: Partial<FaqItem>) {
    setItems((prev) => prev.map((x, idx) => (idx === i ? { ...x, ...patch } : x)));
  }

  async function save() {
    setSaving(true);
    try {
      await saveLayout({
        type: LAYOUT_TYPES.faq,
        faq: items.filter((f) => f.question.trim() && f.answer.trim()),
      });
      toast.success("FAQ saved.");
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardContent className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="font-semibold">FAQ</h3>
          <Button
            variant="secondary"
            size="sm"
            onClick={() => setItems([...items, { question: "", answer: "" }])}
          >
            <Plus className="h-4 w-4" /> Add question
          </Button>
        </div>
        {loading ? (
          <Loader />
        ) : (
          <>
            {items.map((item, i) => (
              <div key={i} className="space-y-2 rounded-xl border border-border p-4">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium">Question {i + 1}</span>
                  {items.length > 1 && (
                    <button
                      onClick={() => setItems(items.filter((_, idx) => idx !== i))}
                      className="text-muted-foreground hover:text-destructive"
                      aria-label="Remove"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  )}
                </div>
                <Input
                  placeholder="Question"
                  value={item.question}
                  onChange={(e) => update(i, { question: e.target.value })}
                />
                <Textarea
                  placeholder="Answer"
                  value={item.answer}
                  onChange={(e) => update(i, { answer: e.target.value })}
                  className="min-h-[70px]"
                />
              </div>
            ))}
            <Button onClick={save} disabled={saving}>
              {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : "Save FAQ"}
            </Button>
          </>
        )}
      </CardContent>
    </Card>
  );
}

/* --------------------------- Categories ---------------------------- */

function CategoriesEditor() {
  const [items, setItems] = useState<string[]>([""]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.categories)
      .then((l) => {
        if (l?.categories?.length) setItems(l.categories.map((c) => c.title));
      })
      .finally(() => setLoading(false));
  }, []);

  async function save() {
    setSaving(true);
    try {
      await saveLayout({
        type: LAYOUT_TYPES.categories,
        categories: items.filter((t) => t.trim()).map((title) => ({ title })),
      });
      toast.success("Categories saved.");
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Card>
      <CardContent className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="font-semibold">Categories</h3>
          <Button variant="secondary" size="sm" onClick={() => setItems([...items, ""])}>
            <Plus className="h-4 w-4" /> Add category
          </Button>
        </div>
        {loading ? (
          <Loader />
        ) : (
          <>
            {items.map((item, i) => (
              <div key={i} className="flex gap-2">
                <Input
                  placeholder="Category title"
                  value={item}
                  onChange={(e) =>
                    setItems(items.map((x, idx) => (idx === i ? e.target.value : x)))
                  }
                />
                {items.length > 1 && (
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setItems(items.filter((_, idx) => idx !== i))}
                    className="shrink-0 text-muted-foreground hover:text-destructive"
                    aria-label="Remove"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                )}
              </div>
            ))}
            <Button onClick={save} disabled={saving}>
              {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : "Save categories"}
            </Button>
          </>
        )}
      </CardContent>
    </Card>
  );
}

function Loader() {
  return (
    <div className="grid min-h-[8rem] place-items-center">
      <Loader2 className="h-5 w-5 animate-spin text-primary" />
    </div>
  );
}
