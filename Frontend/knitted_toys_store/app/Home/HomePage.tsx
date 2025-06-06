"use client";

import React from "react";
import { useRouter } from "next/navigation";
import "./HomePage.css";
import { Button } from "antd";

const HomePage = () => {
  const router = useRouter();

  return (
    <div className="home-container">
      <img
        src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/HomePageImages/Home.png`}
        alt="Главное изображение"
        className="background-image"
        loading="lazy"
        decoding="async"
      />

      <div className="welcome-text">
        Добро пожаловать в магазин вязаных игрушек!
      </div>

      <div className="center-buttons">
        <Button
          className="neon-blue-btn"
          onClick={() => router.push("/toysPage")}
        >
          Каталог
        </Button>
        {/*<button onClick={() => router.push("/promotions")}>Акции</button>*/}
      </div>
    </div>
  );
};

export default HomePage;
