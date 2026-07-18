"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { ArrowRight, PlayCircle, Sparkles } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { getLayout } from "@/lib/services";
import { LAYOUT_TYPES } from "@/lib/types";
import type { Banner } from "@/lib/types";

const DEFAULT_TITLE = "Learn without limits. Build what matters.";
const DEFAULT_SUBTITLE =
  "A premium learning platform where world-class courses meet a beautifully crafted experience. Start your journey today.";

const stats = [
  { value: "120+", label: "Courses" },
  { value: "45k", label: "Learners" },
  { value: "4.9", label: "Avg. rating" },
  { value: "98%", label: "Completion" },
];

export function Hero() {
  // Start from defaults so the hero renders instantly, then hydrate from the Banner layout.
  const [banner, setBanner] = useState<Banner | null>(null);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.banner)
      .then((layout) => {
        if (layout?.banner) setBanner(layout.banner);
      })
      .catch(() => {
        /* keep defaults if the layout API is unavailable */
      });
  }, []);

  const title = banner?.title || DEFAULT_TITLE;
  const subtitle = banner?.subTitle || DEFAULT_SUBTITLE;
  const image = banner?.image?.url;

  return (
    <section className="relative overflow-hidden">
      <div className="absolute inset-0 -z-10 bg-grid" />
      <div className="absolute inset-x-0 top-0 -z-10 h-[500px] bg-brand-radial" />
      <div className="container flex flex-col items-center py-24 text-center md:py-32">
        <Badge className="mb-6 animate-fade-up gap-1.5 py-1.5 pl-2 pr-3">
          <Sparkles className="h-3.5 w-3.5" /> Now powered by .NET 8 + Next.js
        </Badge>
        <h1 className="max-w-4xl animate-fade-up text-balance text-4xl font-bold tracking-tight sm:text-6xl md:text-7xl">
          <HighlightedTitle title={title} isDefault={!banner?.title} />
        </h1>
        <p className="mt-6 max-w-2xl animate-fade-up text-balance text-lg text-muted-foreground">
          {subtitle}
        </p>
        <div className="mt-10 flex animate-fade-up flex-col gap-3 sm:flex-row">
          <Button asChild size="lg">
            <Link href="/courses">
              Explore courses <ArrowRight className="h-4 w-4" />
            </Link>
          </Button>
          <Button asChild size="lg" variant="secondary">
            <Link href="/signup">
              <PlayCircle className="h-4 w-4" /> Get started free
            </Link>
          </Button>
        </div>

        {image ? (
          <div className="mt-16 w-full max-w-4xl animate-fade-up">
            <div className="overflow-hidden rounded-3xl border border-border bg-card shadow-2xl shadow-primary/10">
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={image} alt="" className="aspect-[16/9] w-full object-cover" />
            </div>
          </div>
        ) : (
          <div className="mt-20 grid w-full max-w-3xl animate-fade-up grid-cols-2 gap-4 sm:grid-cols-4">
            {stats.map((s) => (
              <div key={s.label} className="glass rounded-2xl p-5">
                <div className="text-3xl font-bold text-gradient">{s.value}</div>
                <div className="mt-1 text-sm text-muted-foreground">{s.label}</div>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}

/** Renders the default headline with a gradient second clause; custom banner titles render plainly. */
function HighlightedTitle({ title, isDefault }: { title: string; isDefault: boolean }) {
  if (!isDefault) return <>{title}</>;
  const [first, ...rest] = title.split(". ");
  return (
    <>
      {first}. <span className="text-gradient">{rest.join(". ")}</span>
    </>
  );
}
