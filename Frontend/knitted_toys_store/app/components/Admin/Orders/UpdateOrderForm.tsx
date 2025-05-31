"use client";

import {
  addToOrder,
  reduceQuantityItem,
  RemoveItemFromOrder,
  updateStatusOrder,
} from "@/app/services/Admin/serviceOrdersAdmin";
import { getAllToysAdmin } from "@/app/services/Admin/serviceToysAdmin";
import { OrderResponse } from "@/app/types/Order/OrderResponce";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";
import {
  Button,
  Divider,
  Form,
  Input,
  InputNumber,
  message,
  Select,
  Typography,
} from "antd";
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
  const [status, setStatus] = useState(order.status);
  const [changingStatus, setChangingStatus] = useState(false);

  const statusOptions = [
    { value: "Pending", label: "Ожидает платы" },
    { value: "Paid", label: "Оплачен" },
    { value: "Shipped", label: "Отправлен" },
    { value: "Delivered", label: "Доставлен" },
    { value: "Cancelled", label: "Отменён" },
  ];

  useEffect(() => {
    getAllToysAdmin()
      .then((toys) =>
        setToys([...toys].sort((a, b) => a.name.localeCompare(b.name)))
      )
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

  const handleAddItemToOrder = async (toyId: string, quantity: number) => {
    if (!order) return;
    try {
      if (!order.id) {
        console.error("Order ID отсутствует");
        return;
      }
      await addToOrder(order.id, toyId, quantity);
      message.success(`Игрушка ${toyId} успешно добавлена`);
      onSuccess();
    } catch (err) {
      console.error(err);
    }
  };

  const handleAddToy = async () => {
    if (selectedToy && quantity > 0) {
      await handleAddItemToOrder(selectedToy, quantity);
      setSelectedToy(null);
      setQuantity(1);
    }
  };

  const handleReduceQuantityItem = async (toyId: string) => {
    if (!order) return;
    try {
      if (!order.id) {
        console.error("Order ID отсутствует");
        return;
      }
      await reduceQuantityItem(order.id, toyId);
      message.success(`Количество игрушек ${toyId} успешно уменьшено`);
      onSuccess();
    } catch (err) {
      console.error(err);
    }
  };

  const handleChangeStatus = async () => {
    try {
      setChangingStatus(true);
      // вызови API смены статуса
      if (!order.id) {
        console.error("Order ID отсутствует");
        return;
      }
      if (!status) {
        message.error("Статус не выбран");
        setChangingStatus(false);
        return;
      }
      await updateStatusOrder(order.id, status); // реализуй в сервисе
      message.success("Статус заказа обновлён");
      onSuccess(); // обновить форму
    } catch (err) {
      message.error("Не удалось обновить статус");
    } finally {
      setChangingStatus(false);
    }
  };

  const handleRemoveItemFromOrder = async (toyId: string) => {
    if (!order) return;
    if (!order.id) {
      console.error("Cart ID отсутствует");
      return;
    }
    await RemoveItemFromOrder(order.id, toyId);
    onSuccess();
  };

  return (
    <Form layout="vertical" form={form}>
      <Typography.Title level={3}>Характеристики заказа</Typography.Title>
      <Form.Item name="totalAmount" label="Сумма заказа">
        <Input readOnly />
      </Form.Item>
      <Form.Item name="surnameCustomer" label="Фамилия заказчика">
        <Input />
      </Form.Item>
      <Form.Item name="status" label="Статус заказа">
        <Input />
      </Form.Item>
      <Form.Item name="nameCustomer" label="Имя заказчика">
        <Input />
      </Form.Item>
      <Form.Item name="phoneNumber" label="Мобильный номер заказчика">
        <Input />
      </Form.Item>
      <Form.Item name="email" label="Электронная почта заказчика">
        <Input />
      </Form.Item>
      <Form.Item name="deliveryAddress" label="Адрес доставки">
        <Input />
      </Form.Item>
      <Form.Item name="deliveryNotes" label="Примечание к доставке">
        <Input />
      </Form.Item>

      <Divider />

      <Typography.Title level={3}>Позиции в заказе</Typography.Title>
      {order.orderItemsResponse?.length ? (
        order.orderItemsResponse.map((item) => (
          <div
            key={item.toyId}
            style={{ display: "flex", alignItems: "center", marginBottom: 8 }}
          >
            <span>
              {toys?.find((toy) => toy.id === item.toyId)?.name ||
                "Неизвестная игрушка"}{" "}
              — {item.quantity} шт.
            </span>
            <Button onClick={() => handleAddItemToOrder(item.toyId, 1)}>
              +
            </Button>
            <Button onClick={() => handleReduceQuantityItem(item.toyId)}>
              -
            </Button>
            <Button
              type="link"
              danger
              onClick={() => handleRemoveItemFromOrder(item.toyId)}
            >
              Удалить
            </Button>
          </div>
        ))
      ) : (
        <p>Нет товаров в заказе</p>
      )}

      <Divider />
      <Typography.Title level={4}>Изменить статус заказа</Typography.Title>
      <Form.Item label="Статус заказа">
        <Select
          value={status}
          onChange={(value) => setStatus(value)}
          style={{ width: "60%" }}
        >
          {statusOptions.map((option) => (
            <Select.Option key={option.value} value={option.value}>
              {option.label}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Button
        type="primary"
        onClick={handleChangeStatus}
        loading={changingStatus}
      >
        Сохранить статус
      </Button>

      <Typography.Title level={3}>Добавить новую позицию</Typography.Title>
      <Form.Item>
        <Select
          placeholder="Выберите игрушку"
          style={{ width: "60%" }}
          value={selectedToy}
          onChange={(value) => setSelectedToy(value)}
        >
          {toys?.map((toy) => (
            <Select.Option key={toy.id} value={toy.id}>
              {toy.name}
            </Select.Option>
          ))}
        </Select>
        <InputNumber
          min={1}
          value={quantity}
          onChange={(value) => setQuantity(value || 1)}
          style={{ width: "20%" }}
        />
        <Button type="primary" onClick={handleAddToy}>
          Добавить в заказ
        </Button>
      </Form.Item>
    </Form>
  );
};
