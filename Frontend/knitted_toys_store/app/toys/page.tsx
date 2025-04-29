"use client";

import { useEffect, useState } from "react";
import { message } from "antd"; // Импортируем кнопку из Ant Design
import { Toys } from "../components/Toys"; // Импортируем компонент Toys
import { getAllToys } from "../services/toys"; // Импортируем функции для получения игрушек
import { Toy } from "../Models/Toy";
import { addToCart, getCurrentCart } from "../services/carts";
import { CartResponse } from "../types/Cart/CartResponse";
import AdminEasterEgg from "../components/Admin/AdminEasterEgg";

export default function ToysPage() {
  const [ values ] = useState<Toy>({
    name: "",
    description: "",
    size: "",
    price: 1,
    imageUrl: "",
  });

  const [toys, setToys] = useState<Toy[]>([]); // Состояние для списка игрушек
  const [loading, setLoading] = useState(true); // Состояние для загрузки данных
  const [cart, setCart] = useState<CartResponse | null>(null); // Состояние для текущей корзины

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [toysData, cartData] = await Promise.all([
          getAllToys(),
          getCurrentCart(),
        ]);
        setToys(toysData);
        setCart(cartData); // ← сохраняем корзину
      } catch (err) {
        console.error("Ошибка при загрузке данных:", err);
        message.error("Не удалось загрузить данные");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleAddToyToCart = async (idToy: string) => {
    if (!cart) {
      message.error("Корзина не найдена");
      return;
    }
    try {
      await addToCart(cart.id, idToy, 1); // по умолчанию quantity = 1
      message.success("Товар добавлен в корзину");
    } catch (error) {
      console.error("Ошибка при добавлении в корзину:", error);
      message.error("Не удалось добавить товар в корзину");
    }
  };

  useEffect(() => {
    console.log("Полученные игрушки в состоянии:", toys); // Логируем состояние после получения
  }, [toys]); // Следим за изменениями в массиве toys

  return (
    <div>
      <AdminEasterEgg />
      {/* Отображение загруженного изображения */}
      {values.imageUrl && (
        <img
          src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${values.imageUrl}`}
          alt="Загруженное изображение"
          style={{ width: "150px", marginBottom: "10px" }}
        />
      )}

      {/* Отображаем игрушки через компонент Toys */}
      {loading ? (
        <p>Загрузка...</p>
      ) : (
        <Toys 
        toys={toys} 
        onAddToCart={handleAddToyToCart} />
      )}
    </div>
  );
}
