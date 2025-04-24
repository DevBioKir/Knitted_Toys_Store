"use client";

import { useState } from "react";
import { Button, Form, Input, InputNumber, Upload, message } from "antd";
import { UploadOutlined } from "@ant-design/icons";
import { RcFile } from "antd/es/upload";
import { ToyRequest } from "../../types/Toy/ToyRequest";
import { createToy, uploadImage } from "../../services/toys";
import { useRouter } from "next/navigation";

interface AddToyPageProp {
  onToyCreated?: () => void; 
}

export default function AddToyPage({ onToyCreated } : AddToyPageProp) {
  const [imageUrl, setImageUrl] = useState<string>("");
  const [uploading, setUploading] = useState(false);
  const router = useRouter();

  const onFinish = async (values: ToyRequest) => {
    try {
      await createToy({ ...values, imageUrl });
      message.success("Игрушка успешно создана");
      
      if(onToyCreated){
        onToyCreated();
      }else{
        router.push("/toys");
      }
    } catch (error) {
      console.error(error);
      message.error("Ошибка при создании игрушки");
    }
  };

  const handleUpload = async (file: RcFile) => {
    setUploading(true);
    try {
      const path = await uploadImage(file);
      setImageUrl(path);
      message.success("Изображение загружено");
    } catch (error) {
      console.error(error);
      message.error("Не удалось загрузить изображение");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div style={{ maxWidth: 500, margin: "0 auto", padding: 20 }}>
      <h2>Добавить новую игрушку</h2>
      <Form layout="vertical" onFinish={onFinish}>
        <Form.Item name="name" label="Название" rules={[{ required: true, message: "Введите название игрушки" }]}>
          <Input
          placeholder="Имя нового плюшевого друга"/>
        </Form.Item>

        <Form.Item name="description" label="Описание" rules={[{ required: true, message: "Введите описание игрушки" }]}>
          <Input.TextArea rows={3}
          placeholder="Описание материалов, способа вязки"/>
        </Form.Item>

        <Form.Item name="size" label="Размер" rules={[{ required: true, message: "Введите размер игрушки" }]}>
          <Input 
          placeholder="Размер игрушки в мм (110x110)"/>
        </Form.Item>

        <Form.Item name="price" label="Цена" rules={[{ required: true }]}>
          <InputNumber min={1} style={{ width: "100%" }} 
          placeholder="Цена в рублях"/>
        </Form.Item>

        <Form.Item label="Изображение">
          <Upload
            name="image"
            accept="image/*"
            showUploadList={false}
            customRequest={({ file, onSuccess, onError }) => {
                // Защита от отсутствия файла или file с size === 0
                if (!file || !(file instanceof Blob) || (file as File).size === 0) {
                  message.warning("Вы не выбрали файл");
                  return;
                }
              
                handleUpload(file as RcFile)
                  .then(() => onSuccess?.("ok"))
                  .catch((err) =>
                    onError?.({
                      name: "Upload Error",
                      message: err instanceof Error ? err.message : "Ошибка загрузки",
                    })
                  );
              }}
          >
            <Button icon={<UploadOutlined />} loading={uploading}>
              Загрузить изображение
            </Button>
          </Upload>

          {imageUrl && (
            <img
              src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${imageUrl}`}
              alt="Загруженное изображение"
              style={{ marginTop: 10, width: "100%", maxHeight: 200, objectFit: "cover" }}
            />
          )}
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" disabled={!imageUrl}>
            Создать игрушку
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
}