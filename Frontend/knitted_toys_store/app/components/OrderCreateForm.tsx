//import { useRouter } from "next/router";
import { useState } from "react";
import { OrderRequest } from "../types/Order/OrderRequest";
import { createOrder } from "../services/orders";
import { Button, Form, Input, message } from "antd";

interface Prop {
  onOrderCreated?: () => void;
}

export default function OrderCreateForm({ onOrderCreated }: Prop) {
  const [uploading, setUploading] = useState(false);
  //const router = useRouter();

  const onFinish = async (values: OrderRequest) => {
    try {
      await createOrder({ ...values });
      message.success("Заказ успешно создан");
      if (onOrderCreated) {
        onOrderCreated();
      }
    } catch (err) {
      console.error(err);
      message.error("Ошибка при создании заказа");
    }
  };

  return (
    <div style={{ maxWidth: 500, margin: "0 auto", padding: 20 }}>
      <h2>Оформление заказа</h2>
      <Form layout="vertical" onFinish={onFinish}>
        <Form.Item
          label="Фамилия"
          name="surnameCustomer"
          rules={[{ required: true, message: "Введите фамилию" }]}
        >
          <Input placeholder="Введите фамилию" />
        </Form.Item>

        <Form.Item
          label="Имя"
          name="nameCustomer"
          rules={[{ required: true, message: "Введите имя" }]}
        >
          <Input placeholder="Введите имя" />
        </Form.Item>

        <Form.Item
          label="Номер телефона"
          name="phoneNumber"
          rules={[{ required: true, message: "Введите номер телефона" }]}
        >
          <Input placeholder="Введите номер телефона" />
        </Form.Item>

        <Form.Item
          label="Email"
          name="email"
          rules={[
            { required: true, message: "Введите адрес электронной почты" },
          ]}
        >
          <Input placeholder="Введите адрес электронной почты" />
        </Form.Item>

        <Form.Item
          label="Адрес доставки"
          name="deliveryAddress"
          rules={[{ required: true, message: "Введите адрес доставки" }]}
        >
          <Input placeholder="Введите адрес доставки" />
        </Form.Item>

        <Form.Item
          label="Примечание к заказу"
          name="deliveryNotes"
          rules={[{ required: true, message: "Укажите примечние к доставке" }]}
        >
          <Input placeholder="Укажите примечние к доставке" />
        </Form.Item>

        <Button type="primary" htmlType="submit" loading={uploading}>
          Оформить заказ
        </Button>
      </Form>
    </div>
  );
}
