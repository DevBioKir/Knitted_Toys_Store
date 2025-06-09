"use client";

import { useOrder } from "@/app/context/OrderProvider";
import {
  Button,
  Card,
  List,
  Space,
  Typography,
  Spin,
  Tag,
  message,
} from "antd";
import { useRouter } from "next/navigation";
import { statusInfo } from "../components/OrderStatusInfo";
import { updateStatusOrder } from "../services/orders";
import { OrderStatus } from "../Models/Order";
import { useEffect } from "react";
import { deleteOrderAdmin } from "../services/Admin/serviceOrdersAdmin";
import styles from "./OrdersPage.module.css";

const { Title, Text } = Typography;

export default function OrderPage() {
  const { selectedOrder, order, isLoading, refreshOrders, isInitialized } =
    useOrder();
  const router = useRouter();
  const currentOrder = selectedOrder || order;

  useEffect(() => {
    if (!isInitialized) {
      refreshOrders();
    }
  }, [isInitialized]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (!currentOrder) {
    return (
      <div className="text-center p-6">
        <Title level={3}>Нет текущего заказа</Title>
        <Button type="primary" onClick={() => router.push("/toysPage")}>
          Перейти в каталог
        </Button>
      </div>
    );
  }

  if (
    !currentOrder.orderItemsResponse ||
    currentOrder.orderItemsResponse.length === 0
  ) {
    return (
      <div className="text-center p-6">
        <Title level={3}>Заказ пуст</Title>
        <Button type="primary" onClick={() => router.push("/toysPage")}>
          Перейти в каталог
        </Button>
      </div>
    );
  }

  const handlePayOrder = async () => {
    if (!currentOrder?.id) return;
    try {
      await updateStatusOrder(currentOrder.id, OrderStatus.Paid);
      await refreshOrders();
      message.success("Заказ успешно оплачен");
    } catch (err) {
      console.error("Произошла ошибка при оплате заказа", err);
      throw err;
    }
  };

  const handleCancelOrder = async () => {
    if (!currentOrder?.id) return;
    try {
      await updateStatusOrder(currentOrder.id, OrderStatus.Cancelled);
      message.success("Заказ отменён");

      // Через 5 секунд — запрос на удаление
      setTimeout(async () => {
        try {
          if (!currentOrder.id) {
            console.error("ID заказа отсутствует, удаление невозможно");
            return;
          }
          await deleteOrderAdmin(currentOrder.id);
          message.success("Отменённый заказ удалён");
          refreshOrders(); // Обновим, чтобы скрыть удалённый заказ
        } catch (err) {
          console.error("Ошибка при удалении заказа:", err);
          message.error("Ошибка при удалении отменённого заказа");
        }
      }, 5000);

      refreshOrders(); // Сразу обновим, чтобы статус поменялся
    } catch (err) {
      console.error("Ошибка при отмене заказа:", err);
      message.error("Ошибка при отмене заказа");
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <Title level={2}>Текущий заказ</Title>
        <Space>
          <Button onClick={() => router.back()}>Назад</Button>
          <Button onClick={refreshOrders}>Обновить</Button>
        </Space>
      </div>

      <List
        dataSource={currentOrder.orderItemsResponse}
        renderItem={(item) => (
          <Card className={styles.card} key={item.id}>
            <List.Item className={styles.item}>
              <img
                src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${item.toyImageUrl}`}
                alt={item.toyName}
                className={styles.image}
              />
              <div className={styles.itemDetails}>
                <Text strong>{item.toyName}</Text>
                <Text>Количество: {item.quantity}</Text>
                <Text>Цена за штуку: {item.priceAtTime} ₽</Text>
                <Text strong>Итого: {item.quantity * item.priceAtTime} ₽</Text>
              </div>
            </List.Item>
          </Card>
        )}
      />

      <div className={`${styles.statusRow} mt-6`}>
        <Text strong>Статус:</Text>
        <Tag
          color={statusInfo[currentOrder.status]?.color || "default"}
          className="text-lg px-3 py-1"
        >
          {statusInfo[currentOrder.status]?.label || "Неизвестно"}
        </Tag>
      </div>
      <div className={styles.actions}>
        <Text strong className={styles.total}>
          Общая сумма: {currentOrder.totalAmount} ₽
        </Text>
        {currentOrder.status === OrderStatus.Pending && (
          <Button type="primary" onClick={handlePayOrder}>
            Оплатить заказ
          </Button>
        )}
        <Button onClick={handleCancelOrder}>Отменить заказ</Button>
      </div>
    </div>
  );
}
