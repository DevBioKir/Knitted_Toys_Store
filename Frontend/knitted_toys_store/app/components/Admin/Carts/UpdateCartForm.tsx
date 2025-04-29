"use client";

import { Button, Form, Input, InputNumber, message } from "antd";
import { useEffect, useState } from "react";
import { CartRequest } from "@/app/types/Cart/CartRequest";
import { Cart } from "@/app/Models/Cart";
import { updateCartAdmin } from "@/app/services/Admin/serviceCartsAdmin";


interface Props {
  cart: Cart;
  onSuccess: () => void;
}

export const UpdateCartForm = ({ cart, onSuccess }: Props) => {
  const [form] = Form.useForm<CartRequest>();

  useEffect(() => {
    form.setFieldsValue({
      //totalAmount: cart.totalAmount,
      cartItemsRequest: cart.cartItems,
    });
  }, [cart, form]);

  const handleSubmit = async (cartRequest: CartRequest) => {
    try {
      if (!cart.id) {
        message.error("Такого id корзины нет");
        return;
      }
      await updateCartAdmin(cart.id, cartRequest);
      message.success("Корзина обновлена");
      onSuccess();
    } catch (err) {
      console.error(err);
      message.error("Ошибка при обновлении корзины");
    }
  };

  return (
    <Form layout="vertical" form={form} onFinish={handleSubmit}>
      <Form.Item
        name="CartItems"
        label="Состав корзины"
        rules={[{ required: true }]}
      >
        <Input />
      </Form.Item>

      {/* <Form.Item
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
      </Form.Item>*/}

      <Form.Item>
        <Button type="primary" htmlType="submit" block>
          Сохранить
        </Button>
      </Form.Item>
    </Form>
  );
};
