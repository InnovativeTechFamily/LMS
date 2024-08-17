'use client'
import { Suspense } from "react";
import CoursesPage from "../components/Course/CoursesPage";
import Loader from "../components/Loader/Loader";


const Page = () => {
  return (
    <Suspense fallback={<Loader />}>
      <CoursesPage />
    </Suspense>
  );
};

export default Page;
