import Link from "next/link";
import { Logo } from "@/components/logo";

export function Footer() {
  return (
    <footer className="mt-24 border-t border-border/60">
      <div className="container grid gap-10 py-14 md:grid-cols-4">
        <div className="space-y-4">
          <Logo />
          <p className="max-w-xs text-sm text-muted-foreground">
            A premium learning experience — master new skills with expertly crafted courses.
          </p>
        </div>
        {[
          {
            title: "Explore",
            items: [
              { label: "Courses", href: "/courses" },
              { label: "FAQ", href: "/faq" },
              { label: "Get started", href: "/signup" },
            ],
          },
          {
            title: "Company",
            items: [
              { label: "About", href: "/faq" },
              { label: "Careers", href: "/faq" },
              { label: "Contact", href: "/faq" },
            ],
          },
          {
            title: "Legal",
            items: [
              { label: "Privacy", href: "/faq" },
              { label: "Terms", href: "/faq" },
              { label: "Cookies", href: "/faq" },
            ],
          },
        ].map((col) => (
          <div key={col.title} className="space-y-3">
            <h4 className="text-sm font-semibold">{col.title}</h4>
            <ul className="space-y-2 text-sm text-muted-foreground">
              {col.items.map((item) => (
                <li key={item.label}>
                  <Link href={item.href} className="transition-colors hover:text-foreground">
                    {item.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
      <div className="border-t border-border/60">
        <div className="container flex flex-col items-center justify-between gap-2 py-6 text-sm text-muted-foreground md:flex-row">
          <p>© {new Date().getFullYear()} LumeLMS. All rights reserved.</p>
          <p>
            Powered by <span className="text-foreground">.NET 8</span> +{" "}
            <span className="text-foreground">Next.js</span>
          </p>
        </div>
      </div>
    </footer>
  );
}
