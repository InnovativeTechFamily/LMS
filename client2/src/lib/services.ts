import { api } from "./api";
import type { Course, User } from "./types";

/* ----------------------------- Courses ----------------------------- */

export async function getCourses(): Promise<Course[]> {
  const res = await api.get<{ success: boolean; courses: Course[] }>("/get-courses", {
    anonymous: true,
  });
  return res.courses ?? [];
}

export async function getCourse(id: string): Promise<Course | null> {
  const res = await api.get<{ success: boolean; course: Course }>(`/get-course/${id}`, {
    anonymous: true,
  });
  return res.course ?? null;
}

/* ------------------------------ Auth ------------------------------- */

export interface RegisterResult {
  success: boolean;
  message: string;
  activationToken: string;
}

export function register(input: { name: string; email: string; password: string }) {
  return api.post<RegisterResult>("/registration", input, { anonymous: true });
}

export function activate(input: { activation_token: string; activation_code: string }) {
  return api.post<{ success: boolean }>("/activate-user", input, { anonymous: true });
}

export interface LoginResult {
  success: boolean;
  user: User;
  accessToken: string;
}

export function login(input: { email: string; password: string }) {
  return api.post<LoginResult>("/login", input, { anonymous: true });
}

export async function fetchMe(): Promise<User | null> {
  const res = await api.get<{ success: boolean; user: User }>("/me");
  return res.user ?? null;
}

export function logout() {
  return api.get<{ success: boolean }>("/logout");
}

/* ------------------------------ Orders ----------------------------- */

/** Enrolls the current user in a course. Without payment_info the API grants free access. */
export function createOrder(courseId: string) {
  return api.post<{ success: boolean; order: unknown }>("/create-order", { courseId });
}
