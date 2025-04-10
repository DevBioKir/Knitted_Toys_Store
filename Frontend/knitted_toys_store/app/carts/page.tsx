"use client";

import { useEffect, useState } from "react";
import { Button } from "antd";  // Импортируем кнопку из Ant Design
import { Toys } from "../components/Toys"; // Импортируем компонент Toys
import { createToy, getAllToys, updateToy, deleteToy } from "../services/toys"; // Импортируем функции для получения игрушек
import { CreateToyModal } from "../components/CreateToyModal"; // Модалка для создания игрушки
import { UpdateToyModal } from "../components/UpdateToyModal"; // Модалка для редактирования игрушки
import { Cart } from "../Models/Cart";
import { ToyRequest } from "../types/ToyRequest";
import { Mode } from "../components/CreateToy"; // Импортируем Mode

export default function CartsPage() {
  const [values, setValues] = useState<Cart>({
    name: "",
    description: "",
    size: "",
    price: 1,
    imageUrl: "",
  });
