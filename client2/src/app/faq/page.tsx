"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { HelpCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { FaqAccordion } from "@/components/faq-accordion";
import { getLayout } from "@/lib/services";
import { LAYOUT_TYPES } from "@/lib/types";
import type { FaqItem } from "@/lib/types";

const FALLBACK: FaqItem[] = [
  {
    question: "How do I enroll in a course?",
    answer:
      "Open any course from the catalog and click Enroll. Free courses unlock instantly; paid courses take you through a secure checkout. Once enrolled, the course appears under My Learning.",
  },
  {
    question: "Do I get lifetime access?",
    answer:
      "Yes. Once you're enrolled, the course is yours to revisit anytime — including any future updates the instructor publishes.",
  },
  {
    question: "Can I ask questions inside a lecture?",
    answer:
      "Absolutely. Every lecture has a Q&A tab where you can post questions and get answers from instructors and other learners.",
  },
  {
    question: "How do I become an instructor / admin?",
    answer:
      "Admin access is granted from the dashboard by an existing administrator. Reach out to your team's admin to be promoted.",
  },
];

export default function FaqPage() {
  const [items, setItems] = useState<FaqItem[] | null>(null);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.faq)
      .then((layout) => setItems(layout?.faq?.length ? layout.faq : FALLBACK))
      .catch(() => setItems(FALLBACK));
  }, []);

  return (
    <div className="relative">
      <div className="absolute inset-x-0 top-0 -z-10 h-72 bg-brand-radial" />
      <div className="container max-w-3xl py-16 md:py-24">
        <div className="mb-12 text-center">
          <div className="mx-auto mb-5 grid h-14 w-14 place-items-center rounded-2xl bg-primary/15 text-primary">
            <HelpCircle className="h-7 w-7" />
          </div>
          <h1 className="text-3xl font-bold tracking-tight md:text-4xl">
            Frequently asked questions
          </h1>
          <p className="mx-auto mt-3 max-w-xl text-muted-foreground">
            Everything you need to know about learning on LumeLMS. Can&apos;t find an answer?
            Reach out and we&apos;ll help.
          </p>
        </div>

        {items === null ? (
          <div className="grid min-h-[30vh] place-items-center">
            <Loader2 className="h-7 w-7 animate-spin text-primary" />
          </div>
        ) : (
          <FaqAccordion items={items} />
        )}

        <div className="mt-14 rounded-3xl border border-border bg-card p-8 text-center">
          <h2 className="text-xl font-semibold">Still have questions?</h2>
          <p className="mx-auto mt-2 max-w-md text-sm text-muted-foreground">
            Browse the catalog to see what you can learn, or create a free account to get started.
          </p>
          <div className="mt-6 flex flex-col justify-center gap-3 sm:flex-row">
            <Button asChild>
              <Link href="/courses">Explore courses</Link>
            </Button>
            <Button asChild variant="secondary">
              <Link href="/signup">Create free account</Link>
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
