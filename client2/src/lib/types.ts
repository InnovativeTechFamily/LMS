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
}

export interface MediaFile {
  public_id?: string;
  url?: string;
}

export interface TitleItem {
  title: string;
}

export interface Review {
  _id: string;
  user?: Partial<User>;
  rating: number;
  comment: string;
  createdAt?: string;
}

export interface CourseData {
  _id: string;
  title: string;
  description: string;
  videoSection: string;
  videoLength: number;
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
