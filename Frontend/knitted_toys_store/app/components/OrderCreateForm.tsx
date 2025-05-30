import { useState } from "react";
import { OrderRequest } from "../types/Order/OrderRequest";
import { createOrder } from "../services/orders";
import { Button, Form, Input, message } from "antd";
import { useOrder } from "../context/OrderProvider";

interface Prop {
  onOrderCreated?: () => void;
}

export default function OrderCreateForm({ onOrderCreated }: Prop) {
  const [uploading, setUploading] = useState(false);
  const { refreshOrders } = useOrder();

  
  const onFinish = async (values: OrderRequest) => {
  try {
    setUploading(true);

    const result = await createOrder({ ...values });

    if (result.success) {
      await refreshOrders();
      message.success("Заказ успешно создан");
      if (onOrderCreated) {
        onOrderCreated();
      }
    } else if ("warning" in result) {
      message.warning(result.warning); // Показываем предупреждение пользователю
    } else if ("error" in result) {
      message.error(result.error); // Показываем системную ошибку
    }

  } catch (err) {
    console.error(err);
    message.error("Ошибка при соединении с сервером");
  } finally {
    setUploading(false);
  }
};

  
  // const onFinish = async (values: OrderRequest) => {
  //   try {
  //     setUploading(true);
  //     await createOrder({ ...values });
  //     await refreshOrders();
  //     message.success("Заказ успешно создан");
  //     if (onOrderCreated) {
  //       onOrderCreated();
  //     }
  //   } catch (err) {
  //     console.error(err);
  //     message.error("Ошибка при создании заказа");
  //   } finally {
  //     setUploading(false);
  //   }
  // };

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
