"use client";

import { getAllOrdersAdmin } from "@/app/services/Admin/serviceOrdersAdmin";
import { getAllToysAdmin } from "@/app/services/Admin/serviceToysAdmin";
import { OrderRequest } from "@/app/types/Order/OrderRequest";
import { OrderResponse } from "@/app/types/Order/OrderResponce";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";
import { Button, Divider, Form, InputNumber, message, Select, Typography } from "antd";
import { b } from "framer-motion/client";
import { useEffect, useState } from "react";

interface Props {
    order: OrderResponse;
    onSuccess: () => void;
}

export const UpdateOrderForm = ({ order, onSuccess }: Props) => {
  const [form] = Form.useForm();
  const [toys, setToys] = useState<ToyResponse[]>();
  const [selectedToy, setSelectedToy] = useState<string | null>(null);
  const [quantity, setQuantity] = useState<number>(1);

    useEffect(() => {
    getAllToysAdmin()
      .then((toys) => setToys([...toys].sort((a, b) => a.name.localeCompare(b.name))))
      .catch(() => message.error("Не удалось загрузить игрушки"));

    if (order) {
      form.setFieldsValue({
        totalAmount: order.totalAmount,
        status: order.status,
        surnameCustomer: order.surnameCustomer,
        nameCustomer: order.nameCustomer,
        phoneNumber: order.phoneNumber,
        email: order.email,
        deliveryAddress: order.deliveryAddress,
        deliveryNotes: order.deliveryNotes,
        
      });
    }
  }, [order, form]);

return (
    <Form layout="vertical" form={form}>
      <Typography.Title level={3}>Характеристики заказа</Typography.Title>
      <Form.Item name="createAt" label="Дата создания">
        <Input readOnly />
      </Form.Item>
      <Form.Item name="totalAmount" label="Общая сумма">
        <InputNumber style={{ width: "100%" }} readOnly />
      </Form.Item>
      <Form.Item name="rowVersion" label="Версия строки">
        <Input readOnly />
      </Form.Item>

      <Divider />

      <Typography.Title level={3}>Позиции в заказе</Typography.Title>
      {order.orderItems?.length ? (
        order.orderItems.map((item) => (
          <div key={item.toyId} style={{ display: "flex", alignItems: "center", marginBottom: 8 }}>
            <span>{toys?.find((toy) => toy.id === item.toyId)?.name || "Неизвестная игрушка"} — {item.quantity} шт.</span>
            <Button onClick={() => handleAddItem(item.toyId, 1)}>+</Button>
            <Button onClick={() => handleReduceItem(item.toyId)}>-</Button>
            <Button type="link" danger onClick={() => handleRemoveItem(item.toyId)}>Удалить</Button>
          </div>
        ))
      ) : (
        <p>Нет товаров в заказе</p>
      )}

      <Divider />

      <Typography.Title level={3}>Добавить новую позицию</Typography.Title>
      <Form.Item>
        <Select
          placeholder="Выберите игрушку"
          style={{ width: "60%" }}
          value={selectedToy}
          onChange={(value) => setSelectedToy(value)}
        >
          {toys?.map((toy) => (
            <Select.Option key={toy.id} value={toy.id}>{toy.name}</Select.Option>
          ))}
        </Select>
        <InputNumber min={1} value={quantity} onChange={(value) => setQuantity(value || 1)} style={{ width: "20%" }} />
        <Button type="primary" onClick={handleAddToy}>Добавить в заказ</Button>
      </Form.Item>
    </Form>
  );
};