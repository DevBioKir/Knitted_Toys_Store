"use client";

import { Toy } from "@/app/Models/Toy";
import { updateToy, uploadImage } from "@/app/services/toys";
import { ToyRequest } from "@/app/types/Toy/ToyRequest";
import { Button, Form, Input, InputNumber, message } from "antd";
import Upload, { RcFile } from "antd/es/upload";
import { UploadOutlined } from "@ant-design/icons";
import { useEffect, useState } from "react";

interface Props {
  toy: Toy;
  onSuccess: () => void;
}

export const UpdateToyForm = ({ toy, onSuccess }: Props) => {
  const [form] = Form.useForm<ToyRequest>();
  const [imageUrl, setImageUrl] = useState<string>("");
  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    form.setFieldsValue({
      name: toy.name,
      description: toy.description,
      size: toy.size,
      price: toy.price,
      imageUrl: toy.imageUrl,
    });
  }, [toy, form]);

  const handleSubmit = async (toyRequest: ToyRequest) => {
    try {
      if (!toy.id) {
        message.error("Id игрушки отсутствует");
        return;
      }
      await updateToy(toy.id, toyRequest);
      message.success("Игрушка обновлена");
      onSuccess();
    } catch (err) {
      console.error(err);
      message.error("Ошибка при обновлении игрушки");
    }
  };

  const handleImageUpload = async (file: RcFile) => {
    try{
        setUploading(true);
        const pathImage = await uploadImage(file);
        setImageUrl(pathImage);
        form.setFieldValue("imageUrl", pathImage);
        message.success("Изображение загружено");
    } catch (err) {
        console.error(err);
        message.error("Ошибка при загрузке изображения");
    } finally {
        setUploading(false);
    }
  };

  return (
    <Form layout="vertical" form={form} onFinish={handleSubmit}>
      <Form.Item
        name="name"
        label="Название игрушки"
        rules={[{ required: true }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="description"
        label="Описание игрушки"
        rules={[{ required: true }]}
      >
        <Input.TextArea rows={7} />
      </Form.Item>

      <Form.Item
        name="size"
        label="Размер игрушки (110х110)"
        rules={[{ required: true }]}
      >
        <Input />
      </Form.Item>

      <Form.Item name="price" label="Цена игрушки" rules={[{ required: true }]}>
        <InputNumber min={1} style={{ width: "100%" }} />
      </Form.Item>

      <Form.Item label="Загрузить новое изображение">
        <Upload
          showUploadList={false}
          beforeUpload={(file) => {
            handleImageUpload(file);
            return false; // предотвращаем авто-загрузку Upload
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
              style={{ marginTop: 20, width: "80%", maxHeight: 200, objectFit: "cover" }}
            />
          )}
      </Form.Item>

      <Form.Item
        name="imageUrl"
        label="Ссылка на картинку игрушки"
        rules={[{ required: true }]}
      >
        <Input />
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" block>
          Сохранить
        </Button>
      </Form.Item>
    </Form>
  );
};
