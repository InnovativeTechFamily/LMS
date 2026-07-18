"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ArrowRight, Search, Sparkles } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { getLayout } from "@/lib/services";
import { LAYOUT_TYPES } from "@/lib/types";
import type { Banner } from "@/lib/types";

const DEFAULT_TITLE = "Learn without limits. Build what matters.";
const DEFAULT_SUBTITLE =
  "A premium learning platform where world-class courses meet a beautifully crafted experience. Search a topic and start today.";

const TRUST_AVATARS = [
  "https://randomuser.me/api/portraits/women/44.jpg",
  "https://randomuser.me/api/portraits/men/32.jpg",
  "https://randomuser.me/api/portraits/women/68.jpg",
];

export function Hero() {
  const router = useRouter();
  const [banner, setBanner] = useState<Banner | null>(null);
  const [search, setSearch] = useState("");

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

  function onSearch(e: React.FormEvent) {
    e.preventDefault();
    const q = search.trim();
    router.push(q ? `/courses?q=${encodeURIComponent(q)}` : "/courses");
  }

  return (
    <section className="relative overflow-hidden">
      <div className="absolute inset-0 -z-10 bg-grid" />
      <div className="absolute inset-x-0 top-0 -z-10 h-[560px] bg-brand-radial" />

      <div className="container grid items-center gap-12 py-16 md:py-24 lg:grid-cols-2 lg:gap-8">
        {/* Copy + search + trust */}
        <div className="flex flex-col items-center text-center lg:items-start lg:text-left">
          <Badge className="mb-6 animate-fade-up gap-1.5 py-1.5 pl-2 pr-3">
            <Sparkles className="h-3.5 w-3.5" /> Now powered by .NET 8 + Next.js
          </Badge>
          <h1 className="max-w-xl animate-fade-up text-balance text-4xl font-bold tracking-tight sm:text-5xl lg:text-6xl">
            <HighlightedTitle title={title} isDefault={!banner?.title} />
          </h1>
          <p className="mt-5 max-w-lg animate-fade-up text-balance text-lg text-muted-foreground">
            {subtitle}
          </p>

          {/* Search */}
          <form onSubmit={onSearch} className="mt-8 flex w-full max-w-md animate-fade-up items-center">
            <div className="relative flex-1">
              <Search className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground" />
              <input
                type="search"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search courses…"
                aria-label="Search courses"
                className="h-14 w-full rounded-full border border-input bg-background/70 py-3 pl-12 pr-28 text-base shadow-sm outline-none backdrop-blur transition-colors placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-2 focus-visible:ring-ring"
              />
              <Button type="submit" size="sm" className="absolute right-1.5 top-1/2 -translate-y-1/2">
                Search
              </Button>
            </div>
          </form>

          {/* Trust row */}
          <div className="mt-8 flex animate-fade-up items-center gap-3">
            <div className="flex -space-x-3">
              {TRUST_AVATARS.map((src) => (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  key={src}
                  src={src}
                  alt=""
                  className="h-10 w-10 rounded-full border-2 border-background object-cover"
                />
              ))}
            </div>
            <p className="text-sm text-muted-foreground">
              <span className="font-semibold text-foreground">45k+ learners</span> already trust us.{" "}
              <Link href="/courses" className="font-medium text-primary hover:underline">
                View courses
              </Link>
            </p>
          </div>

          <div className="mt-8 hidden animate-fade-up gap-3 sm:flex">
            <Button asChild size="lg">
              <Link href="/courses">
                Explore courses <ArrowRight className="h-4 w-4" />
              </Link>
            </Button>
            <Button asChild size="lg" variant="secondary">
              <Link href="/signup">Get started free</Link>
            </Button>
          </div>
        </div>

        {/* Visual: banner image with animated glow */}
        <div className="relative flex animate-fade-up items-center justify-center">
          <div className="hero-glow absolute aspect-square w-[85%] max-w-[520px] rounded-full" />
          {image ? (
            <div className="relative w-full max-w-[560px] overflow-hidden rounded-3xl border border-border bg-card shadow-2xl shadow-primary/20">
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={image} alt="" className="aspect-[4/3] w-full object-cover" />
            </div>
          ) : (
            <HeroFallbackArt />
          )}
        </div>
      </div>
    </section>
  );
}

/** A branded placeholder visual shown when no banner image is configured. */
function HeroFallbackArt() {
  const stats = [
    { value: "120+", label: "Courses" },
    { value: "4.9", label: "Avg. rating" },
    { value: "98%", label: "Completion" },
    { value: "24/7", label: "Access" },
  ];
  return (
    <div className="relative grid w-full max-w-[520px] grid-cols-2 gap-4">
      {stats.map((s, i) => (
        <div
          key={s.label}
          className={cnCard(i)}
        >
          <div className="text-4xl font-bold text-gradient">{s.value}</div>
          <div className="mt-1 text-sm text-muted-foreground">{s.label}</div>
        </div>
      ))}
    </div>
  );
}

function cnCard(i: number) {
  // Stagger every other card downward for a playful, premium arrangement.
  return `glass rounded-2xl p-6 ${i % 2 === 1 ? "translate-y-6" : ""}`;
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
