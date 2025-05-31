import { useEffect, useState } from "react";
import { ToyRequest } from "../types/Toy/ToyRequest";
import { Toy } from "../Models/Toy";
import { Input, InputNumber, Upload, Button, message } from "antd";
import { UploadOutlined } from "@ant-design/icons";

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
    if (!imageUrl) {
      message.warning("Пожалуйста, загрузите изображение");
      return;
    }

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
        placeholder="Размер игрушки (в мм)"
        style={{ marginBottom: 10 }}
      />
      <InputNumber
        value={price}
        min={1}
        onChange={(value) => setPrice(value || 1)}
        placeholder="Цена"
        style={{ width: "100%", marginBottom: 10 }}
      />

      <Upload
        name="file"
        action={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/ImageUpload/upload`}
        showUploadList={false}
        //onChange={handleUploadChange}
      >
        <Button icon={<UploadOutlined />}>Загрузить изображение</Button>
      </Upload>

      {imageUrl && (
        <img
          src={`${process.env.NEXT_PUBLIC_API_BASE_URL}${imageUrl}`}
          alt="Превью"
          style={{ marginTop: 10, maxWidth: 200, border: "1px solid #ccc" }}
        />
      )}

      <br /><br />
      <Button type="primary" onClick={handleSubmit}>
        Сохранить
      </Button>
    </div>
  );
};