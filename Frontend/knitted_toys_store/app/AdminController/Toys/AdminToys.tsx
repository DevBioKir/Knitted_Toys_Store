"use client";

import { Toy } from "@/app/Models/Toy";
import { deleteToy, getAllToys } from "@/app/services/toys";
import { Button, message } from "antd";
import { useEffect, useState } from "react";
import { UpdateToyForm } from "../../components/Admin/Toys/UpdateToyForm";

export default function AdminToysPage() {
  const [toys, setToys] = useState<Toy[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingToy, setEditingToy] = useState<Toy | null>(null);

  const fetchToys = async () => {
    try {
      const data = await getAllToys();
      setToys(data);
    } catch (err) {
      message.error("Ошибка при загрузке игрушек");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchToys();
  }, []);

  const handleDelete = async (id: string) => {
    try {
      await deleteToy(id);
      message.success("Игрушка удалена");
      fetchToys();
    } catch (err) {
      message.error("Не удалось удалить игрушку");
    }
  };
  return (
    <div style={{ padding: "24px" }}>
      <h2>Игрушки</h2>
      {loading ? (
        <p>Загрузка...</p>
      ) : (
        <>
          {toys.map((toy) => (
            <div
              key={toy.id}
              style={{
                marginBottom: "20px",
                border: "1px solid #ddd",
                borderRadius: "8px",
                padding: "16px",
                display: "flex",
                gap: "16px",
                alignItems: "flex-start",
              }}
            >
              {/* Картинка */}
              {toy.imageUrl ? (
                <img
                  src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${toy.imageUrl}`}
                  alt={toy.name}
                  style={{ width: "120px", height: "120px", objectFit: "cover", borderRadius: "8px" }}
                />
              ) : (
                <div
                  style={{
                    width: "120px",
                    height: "120px",
                    background: "#f0f0f0",
                    borderRadius: "8px",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    color: "#999",
                    fontSize: "12px",
                  }}
                >
                  Нет изображения
                </div>
              )}

              {/* Описание */}
              <div style={{ flexGrow: 1 }}>
                <p><strong>{toy.name}</strong></p>
                <p>Цена: {toy.price}₽</p>
                <p>Описание: {toy.description}</p>
                <p>Размер: {toy.size}мм</p>

                {/* Кнопки */}
                <Button
                  onClick={() => setEditingToy(toy)}
                  style={{ marginRight: 8 }}
                >
                  Редактировать
                </Button>
                <Button onClick={() => handleDelete(toy.id!)}>
                  Удалить
                </Button>
              </div>
            </div>
          ))}
        </>
      )}

      {editingToy && (
        <UpdateToyForm
          toy={editingToy}
          onSuccess={() => {
            setEditingToy(null);
            fetchToys();
          }}
        />
      )}
    </div>
  );
}
