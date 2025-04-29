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
    message.success("Запущена форма обновления корзины");
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

      <Form.Item>
        <Button type="primary" htmlType="submit" block>
          Сохранить
        </Button>
      </Form.Item>
    </Form>
  );
};
