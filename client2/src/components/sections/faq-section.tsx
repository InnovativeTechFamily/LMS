"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { ArrowRight } from "lucide-react";
import { FaqAccordion } from "@/components/faq-accordion";
import { getLayout } from "@/lib/services";
import { LAYOUT_TYPES } from "@/lib/types";
import type { FaqItem } from "@/lib/types";

const FALLBACK: FaqItem[] = [
  {
    question: "How do I enroll in a course?",
    answer:
      "Open any course from the catalog and click Enroll. Free courses unlock instantly, then appear under My Learning.",
  },
  {
    question: "Do I get lifetime access?",
    answer:
      "Yes. Once enrolled, the course is yours to revisit anytime — including future updates from the instructor.",
  },
  {
    question: "Can I ask questions inside a lecture?",
    answer:
      "Every lecture has a Q&A tab where you can post questions and get answers from instructors and other learners.",
  },
  {
    question: "How are payments handled?",
    answer: "Securely via Stripe — your card details never touch our servers.",
  },
];

export function FaqSection() {
  const [items, setItems] = useState<FaqItem[]>(FALLBACK);

  useEffect(() => {
    getLayout(LAYOUT_TYPES.faq)
      .then((l) => {
        if (l?.faq?.length) setItems(l.faq);
      })
      .catch(() => {});
  }, []);

  return (
    <section className="container py-20">
      <div className="mx-auto mb-12 max-w-2xl text-center">
        <p className="mb-2 text-sm font-medium text-primary">Questions?</p>
        <h2 className="text-3xl font-bold tracking-tight md:text-4xl">Frequently asked</h2>
      </div>
      <div className="mx-auto max-w-3xl">
        <FaqAccordion items={items.slice(0, 5)} />
        <div className="mt-6 text-center">
          <Link
            href="/faq"
            className="inline-flex items-center gap-1.5 text-sm font-medium text-primary hover:underline"
          >
            See all FAQs <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      </div>
    </section>
  );
}
