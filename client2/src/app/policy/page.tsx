import type { Metadata } from "next";
import { ScrollText } from "lucide-react";

export const metadata: Metadata = {
  title: "Policies — LumeLMS",
  description: "Privacy Policy, Terms of Service and Cookie Policy for LumeLMS.",
};

const LAST_UPDATED = "July 2026";

interface Section {
  id: string;
  title: string;
  body: { heading?: string; text: string }[];
}

const sections: Section[] = [
  {
    id: "privacy",
    title: "Privacy Policy",
    body: [
      {
        text: "This Privacy Policy explains what information LumeLMS collects, how we use it, and the choices you have. By using the platform you agree to the practices described here.",
      },
      {
        heading: "Information we collect",
        text: "We collect the details you provide when you create an account (name, email, and password, which is always stored hashed), the courses you enrol in, and the questions and reviews you post. We also collect basic technical data such as your IP address for security and rate-limiting.",
      },
      {
        heading: "How we use it",
        text: "Your information is used to operate your account, deliver course content, personalise your learning, process payments, and keep the service secure. We do not sell your personal data.",
      },
      {
        heading: "Payments",
        text: "Payments are handled by our payment processor (Stripe). Card details are entered directly with the processor and never touch our servers; we only store a reference to the transaction.",
      },
      {
        heading: "Your rights",
        text: "You can view and update your profile at any time, change your password, and request deletion of your account. Contact us to exercise any of these rights.",
      },
    ],
  },
  {
    id: "terms",
    title: "Terms of Service",
    body: [
      {
        text: "These Terms govern your use of LumeLMS. If you do not agree with them, please do not use the platform.",
      },
      {
        heading: "Your account",
        text: "You are responsible for keeping your login credentials secure and for all activity under your account. You must provide accurate information and be at least the age of digital consent in your region.",
      },
      {
        heading: "Course access",
        text: "Enrolling in a course grants you a personal, non-transferable licence to access its content for as long as it remains available on the platform. You may not redistribute, resell, or publicly share course materials.",
      },
      {
        heading: "Acceptable use",
        text: "You agree not to misuse the service — including attempting to disrupt it, access it through unauthorised means, or post unlawful, abusive, or infringing content in questions and reviews.",
      },
      {
        heading: "Changes",
        text: "We may update these Terms from time to time. Continued use of the platform after changes take effect constitutes acceptance of the revised Terms.",
      },
    ],
  },
  {
    id: "cookies",
    title: "Cookie Policy",
    body: [
      {
        text: "LumeLMS uses cookies and similar technologies to keep you signed in and to make the platform work reliably.",
      },
      {
        heading: "Essential cookies",
        text: "We use a secure cookie to hold your authentication session so you stay logged in between visits. These are required for the service to function and cannot be disabled.",
      },
      {
        heading: "Local storage",
        text: "We store your access token and theme preference in your browser's local storage so the app can restore your session and appearance instantly.",
      },
      {
        heading: "Managing cookies",
        text: "You can clear cookies and local storage through your browser settings at any time. Doing so will sign you out and reset local preferences.",
      },
    ],
  },
];

export default function PolicyPage() {
  return (
    <div className="relative">
      <div className="absolute inset-x-0 top-0 -z-10 h-64 bg-brand-radial" />
      <div className="container py-16 md:py-20">
        {/* Header */}
        <div className="mb-12 flex flex-col items-center text-center">
          <div className="mb-5 grid h-14 w-14 place-items-center rounded-2xl bg-primary/15 text-primary">
            <ScrollText className="h-7 w-7" />
          </div>
          <h1 className="text-3xl font-bold tracking-tight md:text-4xl">Policies</h1>
          <p className="mt-3 max-w-xl text-muted-foreground">
            The legal bits — how we handle your data, the rules for using LumeLMS, and our use of cookies.
          </p>
          <p className="mt-2 text-xs text-muted-foreground">Last updated: {LAST_UPDATED}</p>
        </div>

        <div className="grid gap-10 lg:grid-cols-[220px_1fr]">
          {/* TOC */}
          <aside className="lg:sticky lg:top-20 lg:self-start">
            <nav className="rounded-2xl border border-border bg-card p-2">
              {sections.map((s) => (
                <a
                  key={s.id}
                  href={`#${s.id}`}
                  className="block rounded-xl px-3 py-2 text-sm text-muted-foreground transition-colors hover:bg-secondary/60 hover:text-foreground"
                >
                  {s.title}
                </a>
              ))}
            </nav>
          </aside>

          {/* Content */}
          <div className="min-w-0 space-y-12">
            {sections.map((s) => (
              <section key={s.id} id={s.id} className="scroll-mt-24">
                <h2 className="mb-4 text-2xl font-bold tracking-tight">{s.title}</h2>
                <div className="space-y-5 rounded-2xl border border-border bg-card p-6 md:p-8">
                  {s.body.map((block, i) => (
                    <div key={i}>
                      {block.heading && (
                        <h3 className="mb-1.5 font-semibold">{block.heading}</h3>
                      )}
                      <p className="text-sm leading-relaxed text-muted-foreground">{block.text}</p>
                    </div>
                  ))}
                </div>
              </section>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
