import Link from "next/link";
import {
  ArrowRight,
  Sparkles,
  Trophy,
  Zap,
  ShieldCheck,
  PlayCircle,
  Infinity as InfinityIcon,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { FeaturedCourses } from "@/components/sections/featured-courses";

const features = [
  {
    icon: Zap,
    title: "Learn at your pace",
    desc: "Bite-sized lessons, lifetime access, and a player that remembers where you left off.",
  },
  {
    icon: Trophy,
    title: "Expert instructors",
    desc: "Courses crafted by practitioners who ship real work, not just theory.",
  },
  {
    icon: ShieldCheck,
    title: "Secure & reliable",
    desc: "Built on a hardened .NET 8 backend with JWT auth and encrypted payments.",
  },
  {
    icon: InfinityIcon,
    title: "Grows with you",
    desc: "From first principles to advanced mastery across a growing catalog.",
  },
];

const stats = [
  { value: "120+", label: "Courses" },
  { value: "45k", label: "Learners" },
  { value: "4.9", label: "Avg. rating" },
  { value: "98%", label: "Completion" },
];

export default function HomePage() {
  return (
    <>
      {/* Hero */}
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 -z-10 bg-grid" />
        <div className="absolute inset-x-0 top-0 -z-10 h-[500px] bg-brand-radial" />
        <div className="container flex flex-col items-center py-24 text-center md:py-32">
          <Badge className="mb-6 animate-fade-up gap-1.5 py-1.5 pl-2 pr-3">
            <Sparkles className="h-3.5 w-3.5" /> Now powered by .NET 8 + Next.js
          </Badge>
          <h1 className="max-w-4xl animate-fade-up text-balance text-4xl font-bold tracking-tight sm:text-6xl md:text-7xl">
            Learn without limits.{" "}
            <span className="text-gradient">Build what matters.</span>
          </h1>
          <p className="mt-6 max-w-2xl animate-fade-up text-balance text-lg text-muted-foreground">
            A premium learning platform where world-class courses meet a beautifully
            crafted experience. Start your journey today.
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

          {/* Stats */}
          <div className="mt-20 grid w-full max-w-3xl animate-fade-up grid-cols-2 gap-4 sm:grid-cols-4">
            {stats.map((s) => (
              <div key={s.label} className="glass rounded-2xl p-5">
                <div className="text-3xl font-bold text-gradient">{s.value}</div>
                <div className="mt-1 text-sm text-muted-foreground">{s.label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="container py-20">
        <div className="mx-auto mb-14 max-w-2xl text-center">
          <p className="mb-2 text-sm font-medium text-primary">Why LumeLMS</p>
          <h2 className="text-3xl font-bold tracking-tight md:text-4xl">
            Everything you need to master a new skill
          </h2>
        </div>
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {features.map((f) => (
            <div
              key={f.title}
              className="group rounded-2xl border border-border bg-card p-6 transition-all hover:-translate-y-1 hover:border-primary/40 hover:shadow-xl hover:shadow-primary/10"
            >
              <div className="mb-4 grid h-12 w-12 place-items-center rounded-xl bg-brand-gradient text-white shadow-lg shadow-primary/30">
                <f.icon className="h-6 w-6" />
              </div>
              <h3 className="text-lg font-semibold">{f.title}</h3>
              <p className="mt-2 text-sm text-muted-foreground">{f.desc}</p>
            </div>
          ))}
        </div>
      </section>

      <FeaturedCourses />

      {/* CTA */}
      <section className="container pb-24">
        <div className="relative overflow-hidden rounded-3xl border border-border bg-brand-gradient p-10 text-center text-white md:p-16">
          <div className="absolute inset-0 bg-grid opacity-20" />
          <div className="relative">
            <h2 className="text-3xl font-bold tracking-tight md:text-4xl">
              Ready to level up?
            </h2>
            <p className="mx-auto mt-3 max-w-xl text-white/80">
              Join thousands of learners building their future — one course at a time.
            </p>
            <Button asChild size="lg" variant="secondary" className="mt-8 text-foreground">
              <Link href="/signup">
                Create your free account <ArrowRight className="h-4 w-4" />
              </Link>
            </Button>
          </div>
        </div>
      </section>
    </>
  );
}
