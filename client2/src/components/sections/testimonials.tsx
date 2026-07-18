import { Quote } from "lucide-react";
import { Ratings } from "@/components/ratings";

const reviews = [
  {
    name: "Gene Bates",
    avatar: "https://randomuser.me/api/portraits/men/1.jpg",
    profession: "Student · Cambridge University",
    comment:
      "I explored the catalog and was thoroughly impressed. The courses cater to every skill level, and the player experience is the best I've used. Highly recommend it to anyone leveling up in tech.",
  },
  {
    name: "Verna Santos",
    avatar: "https://randomuser.me/api/portraits/women/1.jpg",
    profession: "Full-stack Developer · Quarter Ltd.",
    comment:
      "The teaching style is outstanding — complex topics broken into manageable parts with real-world examples. The in-lecture Q&A makes it feel like a supportive classroom.",
  },
  {
    name: "Jay Gibbs",
    avatar: "https://randomuser.me/api/portraits/men/2.jpg",
    profession: "Systems Engineering Student · Zimbabwe",
    comment:
      "Top-notch quality across diverse languages and topics. The practical projects reinforced the theory and gave me the confidence to ship real work.",
  },
  {
    name: "Mina Davidson",
    avatar: "https://randomuser.me/api/portraits/women/2.jpg",
    profession: "Junior Web Developer · Indonesia",
    comment:
      "An extensive range of courses on tech topics, beautifully presented. I learned more here in a month than I did in a semester elsewhere.",
  },
  {
    name: "Rosemary Smith",
    avatar: "https://randomuser.me/api/portraits/women/3.jpg",
    profession: "Full-stack Developer · Algeria",
    comment:
      "The lessons cover everything in detail, so even beginners can complete an integrated project. Thank you — I'm excited for what's next!",
  },
  {
    name: "Laura Mckenzie",
    avatar: "https://randomuser.me/api/portraits/women/4.jpg",
    profession: "Full-stack Developer · Canada",
    comment:
      "Focused on practical applications, not just theory. Building a marketplace with React from start to finish taught me the whole workflow. A great resource.",
  },
];

export function Testimonials() {
  return (
    <section className="container py-20">
      <div className="mx-auto mb-14 max-w-2xl text-center">
        <p className="mb-2 text-sm font-medium text-primary">Loved by learners</p>
        <h2 className="text-3xl font-bold tracking-tight md:text-4xl">
          Our students are <span className="text-gradient">our strength</span>
        </h2>
        <p className="mt-3 text-muted-foreground">
          See what people around the world say about learning with LumeLMS.
        </p>
      </div>

      <div className="columns-1 gap-6 sm:columns-2 lg:columns-3 [&>*]:mb-6">
        {reviews.map((r) => (
          <figure
            key={r.name}
            className="break-inside-avoid rounded-2xl border border-border bg-card p-6 transition-colors hover:border-primary/40"
          >
            <Quote className="h-6 w-6 text-primary/40" />
            <blockquote className="mt-3 text-sm leading-relaxed text-muted-foreground">
              {r.comment}
            </blockquote>
            <div className="mt-5 flex items-center gap-3">
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={r.avatar} alt={r.name} className="h-11 w-11 rounded-full object-cover" />
              <div className="min-w-0">
                <figcaption className="truncate text-sm font-semibold">{r.name}</figcaption>
                <p className="truncate text-xs text-muted-foreground">{r.profession}</p>
              </div>
              <div className="ml-auto">
                <Ratings value={5} />
              </div>
            </div>
          </figure>
        ))}
      </div>
    </section>
  );
}
