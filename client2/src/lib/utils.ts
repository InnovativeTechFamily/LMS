import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatPrice(value?: number) {
  if (value == null) return "Free";
  if (value === 0) return "Free";
  return `₹${value.toLocaleString("en-IN")}`;
}

export function initials(name?: string) {
  if (!name) return "U";
  return name
    .split(" ")
    .map((p) => p[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();
}
