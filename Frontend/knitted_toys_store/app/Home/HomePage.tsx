"use client";

import React from "react";
import { useRouter } from "next/navigation";
import "./HomePage.css";

const HomePage = () => {
  const router = useRouter();

  return (
    <div className="homepage-container">
      <img
        src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/HomePageImages/Home.png`}
        alt="Главное изображение"
        className="background-image"
      />

<div className="welcome-text">Добро пожаловать в магазин вязаных игрушек!</div>

      <div className="center-buttons">
        <button onClick={() => router.push("/toysPage")}>Каталог</button>
        {/*<button onClick={() => router.push("/promotions")}>Акции</button>*/}
      </div>
    </div>
  );
};

export default HomePage;





