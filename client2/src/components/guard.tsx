"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { Loader2, ShieldAlert } from "lucide-react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/providers/auth-provider";

function FullPageLoader() {
  return (
    <div className="grid min-h-[60vh] place-items-center">
      <Loader2 className="h-8 w-8 animate-spin text-primary" />
    </div>
  );
}

/** Renders children only for an authenticated user; otherwise redirects to /login. */
export function RequireAuth({ children }: { children: React.ReactNode }) {
  const { user, loading } = useAuth();
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    if (!loading && !user) {
      router.replace(`/login?redirect=${encodeURIComponent(pathname)}`);
    }
  }, [loading, user, router, pathname]);

  if (loading || !user) return <FullPageLoader />;
  return <>{children}</>;
}

/** Renders children only for an admin; shows a friendly block otherwise. */
export function RequireAdmin({ children }: { children: React.ReactNode }) {
  const { user, loading } = useAuth();
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    if (!loading && !user) {
      router.replace(`/login?redirect=${encodeURIComponent(pathname)}`);
    }
  }, [loading, user, router, pathname]);

  if (loading || !user) return <FullPageLoader />;

  if (user.role !== "admin") {
    return (
      <div className="container grid min-h-[60vh] place-items-center py-20 text-center">
        <div className="max-w-sm">
          <div className="mx-auto mb-5 grid h-14 w-14 place-items-center rounded-full bg-destructive/15 text-destructive">
            <ShieldAlert className="h-7 w-7" />
          </div>
          <h1 className="text-xl font-semibold">Admins only</h1>
          <p className="mt-2 text-sm text-muted-foreground">
            You don&apos;t have permission to view this area.
          </p>
          <Button asChild variant="outline" className="mt-6">
            <Link href="/">Back home</Link>
          </Button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
