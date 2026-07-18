import { api } from "./api";
import type {
  AnalyticsData,
  Course,
  CourseData,
  Notification,
  Order,
  User,
} from "./types";

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

/** Full lecture content for a course the current user is enrolled in. */
export async function getCourseContent(id: string): Promise<CourseData[]> {
  const res = await api.get<{ success: boolean; content: CourseData[] }>(
    `/get-course-content/${id}`
  );
  return res.content ?? [];
}

/* ------------------------------ Q&A -------------------------------- */

export function addQuestion(input: { question: string; courseId: string; contentId: string }) {
  return api.put<{ success: boolean; course: Course }>("/add-question", input);
}

export function addAnswer(input: {
  answer: string;
  courseId: string;
  contentId: string;
  questionId: string;
}) {
  return api.put<{ success: boolean; course: Course }>("/add-answer", input);
}

export function addReview(courseId: string, input: { review: string; rating: number }) {
  return api.put<{ success: boolean; course: Course }>(`/add-review/${courseId}`, input);
}

/** VdoCipher OTP + playbackInfo for embedding a protected video. */
export function getVideoOtp(videoId: string) {
  return api.post<{ otp: string; playbackInfo: string }>(
    "/getVdoCipherOTP",
    { videoId },
    { anonymous: true }
  );
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

/* -------------------------- User profile --------------------------- */

export async function updateUserInfo(input: { name: string }): Promise<User> {
  const res = await api.put<{ success: boolean; user: User }>("/update-user-info", input);
  return res.user;
}

export function updatePassword(input: { oldPassword: string; newPassword: string }) {
  return api.put<{ success: boolean; user: User }>("/update-user-password", input);
}

/** avatar is a base64 data URI. */
export async function updateAvatar(avatar: string): Promise<User> {
  const res = await api.put<{ success: boolean; user: User }>("/update-user-avatar", { avatar });
  return res.user;
}

/* ------------------------------ Orders ----------------------------- */

/** Enrolls the current user in a course. Without payment_info the API grants free access. */
export function createOrder(courseId: string) {
  return api.post<{ success: boolean; order: unknown }>("/create-order", { courseId });
}

/* ============================ Admin only ============================ */

/* --- Users --- */
export async function getUsers(): Promise<User[]> {
  const res = await api.get<{ success: boolean; users: User[] }>("/get-users");
  return res.users ?? [];
}

export async function updateUserRole(input: { email: string; role: string }): Promise<User> {
  const res = await api.put<{ success: boolean; user: User }>("/update-user", input);
  return res.user;
}

export function deleteUser(id: string) {
  return api.del<{ success: boolean }>(`/delete-user/${id}`);
}

/* --- Courses --- */
export async function getAdminCourses(): Promise<Course[]> {
  const res = await api.get<{ success: boolean; courses: Course[] }>("/get-admin-courses");
  return res.courses ?? [];
}

export async function createCourse(input: unknown): Promise<Course> {
  const res = await api.post<{ success: boolean; course: Course }>("/create-course", input);
  return res.course;
}

export async function editCourse(id: string, input: unknown): Promise<Course> {
  const res = await api.put<{ success: boolean; course: Course }>(`/edit-course/${id}`, input);
  return res.course;
}

export function deleteCourse(id: string) {
  return api.del<{ success: boolean }>(`/delete-course/${id}`);
}

/* --- Orders --- */
export async function getOrders(): Promise<Order[]> {
  const res = await api.get<{ success: boolean; orders: Order[] }>("/get-orders");
  return res.orders ?? [];
}

/* --- Analytics --- */
export async function getUsersAnalytics(): Promise<AnalyticsData> {
  const res = await api.get<{ success: boolean; users: AnalyticsData }>("/get-users-analytics");
  return res.users;
}

export async function getOrdersAnalytics(): Promise<AnalyticsData> {
  const res = await api.get<{ success: boolean; orders: AnalyticsData }>("/get-orders-analytics");
  return res.orders;
}

export async function getCoursesAnalytics(): Promise<AnalyticsData> {
  const res = await api.get<{ success: boolean; courses: AnalyticsData }>("/get-courses-analytics");
  return res.courses;
}

/* --- Notifications --- */
export async function getNotifications(): Promise<Notification[]> {
  const res = await api.get<{ success: boolean; notifications: Notification[] }>(
    "/get-all-notifications"
  );
  return res.notifications ?? [];
}

export async function markNotificationRead(id: string): Promise<Notification[]> {
  const res = await api.put<{ success: boolean; notifications: Notification[] }>(
    `/update-notification/${id}`
  );
  return res.notifications ?? [];
}
