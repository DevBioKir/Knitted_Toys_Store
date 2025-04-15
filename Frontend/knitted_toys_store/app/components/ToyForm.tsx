import { useEffect, useState } from "react";
import { ToyRequest } from "../types/Toy/ToyRequest";
import { Toy } from "../Models/Toy";
import { Input, InputNumber, message } from "antd";

interface ToyFormProp {
  initialValues?: Toy;
  onSubmit: (data: ToyRequest) => void;
}

export const ToyForm = ({ initialValues, onSubmit }: ToyFormProp) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [size, setSize] = useState("");
  const [price, setPrice] = useState(1);
  const [imageUrl, setImageUrl] = useState("");
  const [imageFile, setImageFile] = useState<File | null>(null);

  useEffect(() => {
    if (initialValues) {
      setName(initialValues.name);
      setDescription(initialValues.description);
      setSize(initialValues.size);
      setPrice(initialValues.price);
      setImageUrl(initialValues.imageUrl);
    }
  }, [initialValues]);

  const uploadImage = async (file: File): Promise<string | null> => {
    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch("http://localhost:5000/api/Image/UploadImage", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) throw new Error("Upload failed");

      const data = await response.json();
      return data.imageUrl; // e.g., /Images/abc.jpg
    } catch (err) {
      console.error("Ошибка загрузки файла", err);
      message.error("Не удалось загрузить изображение");
      return null;
    }
  };

  const handleSubmit = async () => {
    let finalImageUrl = imageUrl;

    if (imageFile) {
      const uploaded = await uploadImage(imageFile);
      if (!uploaded) return;
      finalImageUrl = uploaded;
    }

    onSubmit({ name, description, size, price, imageUrl: finalImageUrl });
  };

  return (
    <div className="toy_form">
      <Input
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Имя игрушки"
        style={{ marginBottom: 10 }}
      />
      <Input
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        placeholder="Описание игрушки"
        style={{ marginBottom: 10 }}
      />
      <Input
        value={size}
        onChange={(e) => setSize(e.target.value)}
        placeholder="Размер игрушки(указывать в мм)"
        style={{ marginBottom: 10 }}
      />
      <InputNumber
        value={price}
        min={1}
        onChange={(value) => setPrice(value || 1)}  // защитимся от null
        placeholder={"Цена"}
        style={{ width: "100%", marginBottom: 10 }}
      />
      <Input
        value={imageUrl}
        onChange={(e) => setImageUrl(e.target.value)}
        placeholder="Ссылка на изображение игрушки"
        style={{ marginBottom: 10 }}
      />
      <br /><br />
      <button onClick={handleSubmit}>Сохранить</button>
    </div>
  );
};
