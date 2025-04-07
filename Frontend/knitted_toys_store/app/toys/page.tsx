"use client";
import { useEffect, useState } from "react";
import { Button } from "antd";  // Импортируем кнопку из Ant Design
import { Toys } from "../components/Toys"; // Импортируем компонент Toys
import { getAllToys } from "../services/toys"; // Импортируем функцию для получения игрушек

export default function ToysPage() {
  const [toys, setToys] = useState<Toy[]>([]);  // Состояние для списка игрушек
  const [loading, setLoading] = useState(true);  // Состояние для загрузки данных

  useEffect(() => {
    const getToys = async () => {
      const toys = await getAllToys();
      setLoading(false);
      setToys(toys);
    };

    getToys();
  }, []);  // Хук запускается только один раз при монтировании компонента

  useEffect(() => {
    console.log("Полученные игрушки в состоянии:", toys); // Логируем состояние после получения
  }, [toys]); // Следим за изменениями в массиве toys

  return (
    <div>
      {/* Кнопка "Добавить игрушку" */}
      <Button type="primary" style={{ marginBottom: "20px" }}>
        Добавить игрушку
      </Button>

      {/* Отображаем игрушки через компонент Toys */}
      {loading ? <p>Загрузка...</p> : <Toys toys={toys} />}
    </div>
  );
}