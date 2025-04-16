"use client";

import { useEffect, useState } from "react";
import { Button } from "antd";  // Импортируем кнопку из Ant Design
import { Toys } from "../components/Toys"; // Импортируем компонент Toys
import { createToy, getAllToys, updateToy, deleteToy, uploadImage } from "../services/toys"; // Импортируем функции для получения игрушек
import { CreateToyModal } from "../components/CreateToyModal"; // Модалка для создания игрушки
import { UpdateToyModal } from "../components/UpdateToyModal"; // Модалка для редактирования игрушки
import { Toy } from "../Models/Toy";
import { ToyRequest } from "../types/Toy/ToyRequest";
import { Mode } from "../components/CreateToy"; // Импортируем Mode

export default function ToysPage() {
  const [values, setValues] = useState<Toy>({
    name: "",
    description: "",
    size: "",
    price: 1,
    imageUrl: "",
  });

  const [toys, setToys] = useState<Toy[]>([]);  // Состояние для списка игрушек
  const [loading, setLoading] = useState(true);  // Состояние для загрузки данных
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [mode, setMode] = useState<Mode>(Mode.Create); // Добавляем режим (создание или редактирование)
  const [toyToEdit, setToyToEdit] = useState<Toy | null>(null); // Для редактирования игрушки

  useEffect(() => {
    const getToys = async () => {
      const toys = await getAllToys();
      setLoading(false);
      setToys(toys);
    };

    getToys();
  }, []);  // Хук запускается только один раз при монтировании компонента

  // Функция для создания игрушки
  const handleCreate = async (toyRequest: ToyRequest) => {
    try {
      await createToy(toyRequest);  // Вызываем сервис для создания игрушки
      const updatedToys = await getAllToys();  // Обновляем список игрушек после создания
      setToys(updatedToys);
      setIsModalOpen(false); // Закрываем модалку после создания
    } catch (error) {
      console.error("Ошибка при создании игрушки", error);
    }
  };

  // Функция для обновления игрушки
  const handleUpdate = async (id: string, toyRequest: ToyRequest) => {
    try {
      await updateToy(id, toyRequest);  // Вызываем сервис для обновления игрушки
      const updatedToys = await getAllToys();  // Обновляем список игрушек после обновления
      setToys(updatedToys);
      setIsModalOpen(false); // Закрываем модалку после обновления
    } catch (error) {
      console.error("Ошибка при обновлении игрушки", error);
    }
  };

  // Функция для удаления игрушки
  const handleDelete = async (id: string) => {
    try {
      await deleteToy(id);  // Удаляем игрушку
      const updatedToys = await getAllToys();  // Обновляем список игрушек
      setToys(updatedToys);
    } catch (error) {
      console.error("Ошибка при удалении игрушки", error);
    }
  };

  // Функция для открытия модалки в режиме создания
  const openCreateModal = () => {
    setMode(Mode.Create);
    setValues({
      name: "",
      description: "",
      size: "",
      price: 1,
      imageUrl: "",
    });
    setIsModalOpen(true);
  };

  // Функция для открытия модалки в режиме редактирования
  const openUpdateModal = (toy: Toy) => {
    setMode(Mode.Update);
    setToyToEdit(toy); // Устанавливаем toy для редактирования
    setIsModalOpen(true);
  };

  useEffect(() => {
    console.log("Полученные игрушки в состоянии:", toys); // Логируем состояние после получения
  }, [toys]); // Следим за изменениями в массиве toys

  return (
    <div>
      {/* Поле загрузки изображения */}
      <input
        type="file"
        accept="image/*"
        onChange={async (e) => {
          const file = e.target.files?.[0];
          if (file) {
            try {
              const url = await uploadImage(file);
              setValues((prev) => ({
                ...prev,
                imageUrl: url,
              }));
            } catch (err) {
              console.error("Ошибка загрузки изображения", err);
            }
          }
        }}
        style={{ marginBottom: "10px" }}
      />

      {/* Отображение загруженного изображения */}
      {values.imageUrl && (
        <img
          src={`http://localhost:5237${values.imageUrl}`}
          alt="Загруженное изображение"
          style={{ width: "150px", marginBottom: "10px" }}
        />
      )}
  
      {/* Кнопка для создания новой игрушки */}
      <Button
        type="primary"
        style={{ marginBottom: "20px" }}
        onClick={openCreateModal}
      >
        Добавить игрушку
      </Button>

      {/* Модалка для создания игрушки */}
      {mode === Mode.Create && (
        <CreateToyModal
          isOpen={isModalOpen}
          onCancel={() => setIsModalOpen(false)}
          onCreate={handleCreate}
        />
      )}

      {/* Модалка для редактирования игрушки */}
      {mode === Mode.Update && toyToEdit && (
        <UpdateToyModal
          toy={toyToEdit}
          isOpen={isModalOpen}
          onCancel={() => setIsModalOpen(false)}
          onUpdate={handleUpdate}
        />
      )}

      {/* Отображаем игрушки через компонент Toys */}
      {loading ? <p>Загрузка...</p> : <Toys toys={toys} onEdit={openUpdateModal} onDelete={handleDelete} />}
    </div>
  );
}