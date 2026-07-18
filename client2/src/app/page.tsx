import Link from "next/link";
import {
  ArrowRight,
  Trophy,
  Zap,
  ShieldCheck,
  Infinity as InfinityIcon,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Hero } from "@/components/sections/hero";
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

export default function HomePage() {
  return (
    <>
      <Hero />

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
