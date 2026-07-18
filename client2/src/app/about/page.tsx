import type { Metadata } from "next";
import Link from "next/link";
import {
  ArrowRight,
  Compass,
  Heart,
  Rocket,
  ShieldCheck,
  Sparkles,
  Users,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";

export const metadata: Metadata = {
  title: "About — LumeLMS",
  description:
    "LumeLMS is a premium learning platform built to make world-class education feel effortless — powered by .NET 8 and Next.js.",
};

const values = [
  {
    icon: Compass,
    title: "Learning that sticks",
    desc: "Bite-sized lectures, in-context Q&A, and a player that remembers where you left off — designed around how people actually learn.",
  },
  {
    icon: Heart,
    title: "Crafted with care",
    desc: "Every screen is considered. A calm, premium interface keeps the focus on the content, not the chrome.",
  },
  {
    icon: ShieldCheck,
    title: "Trust by default",
    desc: "A hardened .NET 8 backend with JWT auth and encrypted payments keeps your account and data safe.",
  },
  {
    icon: Rocket,
    title: "Built to grow",
    desc: "From first principles to advanced mastery, the catalog and platform evolve with you.",
  },
];

const stats = [
  { value: "120+", label: "Courses" },
  { value: "45k", label: "Learners" },
  { value: "4.9", label: "Avg. rating" },
  { value: "98%", label: "Completion" },
];

export default function AboutPage() {
  return (
    <div className="relative">
      {/* Hero */}
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 -z-10 bg-grid" />
        <div className="absolute inset-x-0 top-0 -z-10 h-[420px] bg-brand-radial" />
        <div className="container flex flex-col items-center py-20 text-center md:py-28">
          <Badge className="mb-6 gap-1.5 py-1.5 pl-2 pr-3">
            <Sparkles className="h-3.5 w-3.5" /> Our story
          </Badge>
          <h1 className="max-w-3xl text-balance text-4xl font-bold tracking-tight sm:text-5xl md:text-6xl">
            We believe learning should feel{" "}
            <span className="text-gradient">effortless</span>.
          </h1>
          <p className="mt-6 max-w-2xl text-balance text-lg text-muted-foreground">
            LumeLMS started with a simple idea: great courses deserve a great home. We&apos;re
            building a learning platform that treats both learners and instructors with respect —
            fast, beautiful, and genuinely useful.
          </p>
        </div>
      </section>

      {/* Mission */}
      <section className="container pb-4">
        <div className="mx-auto max-w-3xl rounded-3xl border border-border bg-card p-8 text-center md:p-12">
          <h2 className="text-2xl font-bold tracking-tight md:text-3xl">Our mission</h2>
          <p className="mt-4 text-muted-foreground">
            To make world-class education accessible and delightful — removing every bit of friction
            between a curious mind and the skill it&apos;s reaching for. We measure success not in
            hours watched, but in things built and careers moved forward.
          </p>
        </div>
      </section>

      {/* Values */}
      <section className="container py-16">
        <div className="mx-auto mb-12 max-w-2xl text-center">
          <p className="mb-2 text-sm font-medium text-primary">What we stand for</p>
          <h2 className="text-3xl font-bold tracking-tight md:text-4xl">The principles behind LumeLMS</h2>
        </div>
        <div className="grid gap-6 sm:grid-cols-2">
          {values.map((v) => (
            <div
              key={v.title}
              className="group rounded-2xl border border-border bg-card p-6 transition-all hover:-translate-y-1 hover:border-primary/40 hover:shadow-xl hover:shadow-primary/10"
            >
              <div className="mb-4 grid h-12 w-12 place-items-center rounded-xl bg-brand-gradient text-white shadow-lg shadow-primary/30">
                <v.icon className="h-6 w-6" />
              </div>
              <h3 className="text-lg font-semibold">{v.title}</h3>
              <p className="mt-2 text-sm text-muted-foreground">{v.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Stats */}
      <section className="container pb-16">
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
          {stats.map((s) => (
            <div key={s.label} className="glass rounded-2xl p-6 text-center">
              <div className="text-3xl font-bold text-gradient">{s.value}</div>
              <div className="mt-1 text-sm text-muted-foreground">{s.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* Tech / trust strip */}
      <section className="container pb-16">
        <div className="flex flex-col items-center gap-3 rounded-2xl border border-border bg-card p-8 text-center">
          <Users className="h-6 w-6 text-primary" />
          <p className="max-w-xl text-sm text-muted-foreground">
            Built by a small team that cares about the details, on a modern stack —{" "}
            <span className="font-medium text-foreground">.NET 8</span> Clean Architecture on the
            backend and <span className="font-medium text-foreground">Next.js</span> on the front.
          </p>
        </div>
      </section>

      {/* CTA */}
      <section className="container pb-24">
        <div className="relative overflow-hidden rounded-3xl border border-border bg-brand-gradient p-10 text-center text-white md:p-16">
          <div className="absolute inset-0 bg-grid opacity-20" />
          <div className="relative">
            <h2 className="text-3xl font-bold tracking-tight md:text-4xl">Come learn with us</h2>
            <p className="mx-auto mt-3 max-w-xl text-white/80">
              Join thousands of learners building their future — one course at a time.
            </p>
            <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
              <Button asChild size="lg" variant="secondary" className="text-foreground">
                <Link href="/signup">
                  Create your free account <ArrowRight className="h-4 w-4" />
                </Link>
              </Button>
              <Button
                asChild
                size="lg"
                variant="outline"
                className="border-white/40 bg-transparent text-white hover:bg-white/10"
              >
                <Link href="/courses">Browse courses</Link>
              </Button>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
