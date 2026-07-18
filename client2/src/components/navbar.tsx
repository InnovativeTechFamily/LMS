"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { LayoutDashboard, LogOut, Menu, User as UserIcon, GraduationCap, X } from "lucide-react";
import { Logo } from "@/components/logo";
import { ThemeToggle } from "@/components/theme-toggle";
import { Button } from "@/components/ui/button";
import { Avatar } from "@/components/ui/avatar";
import { useAuth } from "@/providers/auth-provider";
import { cn } from "@/lib/utils";

const links = [
  { href: "/", label: "Home" },
  { href: "/courses", label: "Courses" },
  { href: "/faq", label: "FAQ" },
];

export function Navbar() {
  const pathname = usePathname();
  const { user, logout, loading } = useAuth();
  const [open, setOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function onClick(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) setMenuOpen(false);
    }
    document.addEventListener("mousedown", onClick);
    return () => document.removeEventListener("mousedown", onClick);
  }, []);

  // Close menus on route change.
  useEffect(() => {
    setMenuOpen(false);
    setOpen(false);
  }, [pathname]);

  const isAdmin = user?.role === "admin";

  return (
    <header className="sticky top-0 z-50 border-b border-border/60 bg-background/70 backdrop-blur-xl">
      <div className="container flex h-16 items-center justify-between gap-4">
        <div className="flex items-center gap-8">
          <Logo />
          <nav className="hidden items-center gap-1 md:flex">
            {links.map((l) => (
              <Link
                key={l.href}
                href={l.href}
                className={cn(
                  "rounded-full px-4 py-2 text-sm font-medium transition-colors",
                  pathname === l.href
                    ? "bg-secondary text-foreground"
                    : "text-muted-foreground hover:text-foreground"
                )}
              >
                {l.label}
              </Link>
            ))}
          </nav>
        </div>

        <div className="flex items-center gap-2">
          <ThemeToggle />
          {!loading && user ? (
            <div className="relative hidden md:block" ref={menuRef}>
              <button
                onClick={() => setMenuOpen((v) => !v)}
                className="flex items-center gap-2 rounded-full py-1 pl-1 pr-3 transition-colors hover:bg-secondary/60"
                aria-label="Account menu"
              >
                <Avatar name={user.name} url={user.avatar?.url} className="h-9 w-9 text-sm" />
                <span className="text-sm font-medium">{user.name.split(" ")[0]}</span>
              </button>

              {menuOpen && (
                <div className="absolute right-0 mt-2 w-56 overflow-hidden rounded-2xl border border-border bg-card p-1.5 shadow-2xl">
                  <div className="px-3 py-2">
                    <p className="truncate text-sm font-medium">{user.name}</p>
                    <p className="truncate text-xs text-muted-foreground">{user.email}</p>
                  </div>
                  <div className="my-1 h-px bg-border" />
                  <MenuLink href="/profile" icon={<UserIcon className="h-4 w-4" />}>
                    Profile
                  </MenuLink>
                  <MenuLink href="/my-courses" icon={<GraduationCap className="h-4 w-4" />}>
                    My Learning
                  </MenuLink>
                  {isAdmin && (
                    <MenuLink href="/admin" icon={<LayoutDashboard className="h-4 w-4" />}>
                      Admin Dashboard
                    </MenuLink>
                  )}
                  <div className="my-1 h-px bg-border" />
                  <button
                    onClick={() => logout()}
                    className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm text-destructive transition-colors hover:bg-destructive/10"
                  >
                    <LogOut className="h-4 w-4" /> Log out
                  </button>
                </div>
              )}
            </div>
          ) : (
            <div className="hidden items-center gap-2 md:flex">
              <Button asChild variant="ghost" size="sm">
                <Link href="/login">Log in</Link>
              </Button>
              <Button asChild size="sm">
                <Link href="/signup">Get started</Link>
              </Button>
            </div>
          )}

          <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            aria-label="Menu"
            onClick={() => setOpen((v) => !v)}
          >
            {open ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
          </Button>
        </div>
      </div>

      {open && (
        <div className="border-t border-border/60 bg-background md:hidden">
          <nav className="container flex flex-col gap-1 py-4">
            {links.map((l) => (
              <Link
                key={l.href}
                href={l.href}
                className="rounded-lg px-4 py-2 text-sm font-medium hover:bg-secondary"
              >
                {l.label}
              </Link>
            ))}
            {user && (
              <>
                <Link href="/profile" className="rounded-lg px-4 py-2 text-sm font-medium hover:bg-secondary">
                  Profile
                </Link>
                <Link href="/my-courses" className="rounded-lg px-4 py-2 text-sm font-medium hover:bg-secondary">
                  My Learning
                </Link>
                {isAdmin && (
                  <Link href="/admin" className="rounded-lg px-4 py-2 text-sm font-medium hover:bg-secondary">
                    Admin Dashboard
                  </Link>
                )}
              </>
            )}
            <div className="mt-2 flex gap-2 px-1">
              {user ? (
                <Button className="flex-1" variant="secondary" onClick={() => logout()}>
                  Log out
                </Button>
              ) : (
                <>
                  <Button asChild variant="secondary" className="flex-1">
                    <Link href="/login">Log in</Link>
                  </Button>
                  <Button asChild className="flex-1">
                    <Link href="/signup">Get started</Link>
                  </Button>
                </>
              )}
            </div>
          </nav>
        </div>
      )}
    </header>
  );
}

function MenuLink({
  href,
  icon,
  children,
}: {
  href: string;
  icon: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <Link
      href={href}
      className="flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm transition-colors hover:bg-secondary/60"
    >
      {icon}
      {children}
    </Link>
  );
}
