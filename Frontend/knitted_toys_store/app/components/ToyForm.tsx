import { useEffect, useState } from "react";
import { ToyRequest } from "../types/ToyRequest";
import { Toy } from "../Models/Toy";
import { Input, InputNumber } from "antd";

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

  useEffect(() => {
    if (initialValues) {
      setName(initialValues.name);
      setDescription(initialValues.description);
      setSize(initialValues.size);
      setPrice(initialValues.price);
      setImageUrl(initialValues.imageUrl);
    }
  }, [initialValues]);

  const handleSubmit = () => {
    onSubmit({ name, description, size, price, imageUrl });
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
