import Link from "next/link";
import { GraduationCap } from "lucide-react";
import { cn } from "@/lib/utils";

export function Logo({ className }: { className?: string }) {
  return (
    <Link href="/" className={cn("flex items-center gap-2 font-semibold", className)}>
      <span className="grid h-9 w-9 place-items-center rounded-xl bg-brand-gradient text-white shadow-lg shadow-primary/30">
        <GraduationCap className="h-5 w-5" />
      </span>
      <span className="text-lg tracking-tight">
        Lume<span className="text-gradient">LMS</span>
      </span>
    </Link>
  );
}
