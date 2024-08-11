"use client";
import React, { FC, useEffect, useState } from "react";
import Heading from "./utils/Heading";
import Header from "./components/Header";
import Hero from "./components/Route/Hero";
import Footer from "./components/Footer";

interface Props {}

const Page: FC<Props> = (props) => {
  const [open, setOpen] = useState(false); // Added state for `open`
  const [activeItem, setActiveItem] = useState(0); // Added state for `activeItem`
  const [route,setRoute] = useState("Login");

  return (
    <div>
      <Heading
        title="ELearning - Admin"
        description="ELearning is a platform for students to learn and get help from teachers"
        keywords="Prograaming,MERN,Redux,Machine Learning"
      />
      <Header open={open} setOpen={setOpen} activeItem={activeItem} setRoute={setRoute} route={route} />
      <Hero/>
      <Footer />
    </div>

  );
};

export default Page;
