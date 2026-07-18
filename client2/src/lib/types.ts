export interface Avatar {
  public_id?: string;
  url?: string;
}

export interface User {
  _id: string;
  name: string;
  email: string;
  role: string;
  isVerified: boolean;
  avatar?: Avatar;
  courses: { courseId: string }[];
  createdAt?: string;
}

export interface MediaFile {
  public_id?: string;
  url?: string;
}

export interface TitleItem {
  title: string;
}

export interface Link {
  title: string;
  url: string;
}

/** Denormalised user snapshot embedded in questions/reviews/replies. */
export interface UserSummary {
  _id?: string;
  name?: string;
  email?: string;
  role?: string;
  avatar?: Avatar;
}

export interface CommentReply {
  _id: string;
  user?: UserSummary;
  answer: string;
  createdAt?: string;
}

/** A learner question on a lecture, with threaded replies. */
export interface Comment {
  _id: string;
  user?: UserSummary;
  question: string;
  questionReplies: CommentReply[];
  createdAt?: string;
}

export interface ReviewReply {
  _id: string;
  user?: UserSummary;
  comment: string;
  createdAt?: string;
}

export interface Review {
  _id: string;
  user?: UserSummary;
  rating: number;
  comment: string;
  commentReplies?: ReviewReply[];
  createdAt?: string;
}

export interface CourseData {
  _id: string;
  title: string;
  description: string;
  videoUrl: string;
  videoSection: string;
  videoLength: number;
  videoPlayer: string;
  links: Link[];
  suggestion: string;
  questions: Comment[];
}

export interface Course {
  _id: string;
  name: string;
  description: string;
  categories: string;
  price: number;
  estimatedPrice?: number;
  thumbnail?: MediaFile;
  tags: string;
  level: string;
  demoUrl: string;
  benefits: TitleItem[];
  prerequisites: TitleItem[];
  reviews: Review[];
  courseData: CourseData[];
  ratings: number;
  purchased: number;
  createdAt?: string;
}

export interface Notification {
  _id: string;
  title: string;
  message: string;
  status: string;
  userId?: string;
  createdAt?: string;
}

export interface Order {
  _id: string;
  courseId: string;
  userId: string;
  payment_info?: Record<string, unknown>;
  createdAt?: string;
}

export interface MonthData {
  month: string;
  count: number;
}

export interface AnalyticsData {
  last12Months: MonthData[];
}

/* ------------------------------ Layout ----------------------------- */

export interface FaqItem {
  question: string;
  answer: string;
}

export interface Category {
  title: string;
}

export interface Banner {
  image?: MediaFile;
  title: string;
  subTitle: string;
}

export interface Layout {
  _id?: string;
  type: string;
  faq: FaqItem[];
  categories: Category[];
  banner?: Banner;
}

export const LAYOUT_TYPES = {
  banner: "Banner",
  faq: "FAQ",
  categories: "Categories",
} as const;
