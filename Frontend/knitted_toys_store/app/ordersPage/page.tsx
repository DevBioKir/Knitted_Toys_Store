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

const { Title, Text } = Typography;

export default function OrderPage() {
  const { selectedOrder, order, isLoading, refreshOrders } = useOrder();

  const router = useRouter();

  const currentOrder = selectedOrder || order;

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (!currentOrder || currentOrder.orderItemsResponse?.length === 0) {
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

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <Title level={2}>Текущий заказ</Title>
        <Space>
          <Button onClick={() => router.back()}>Назад</Button>
          <Button onClick={refreshOrders}>Обновить</Button>
        </Space>
      </div>

      <List
        dataSource={currentOrder.orderItemsResponse}
        renderItem={(item) => (
          <Card className="mb-4" key={item.id}>
            <List.Item>
              <List.Item.Meta
                avatar={
                  <img
                    src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${item.toyImageUrl}`}
                    alt={item.toyName}
                    style={{ width: 80, height: 80, objectFit: "cover" }}
                  />
                }
                title={<Text strong>{item.toyName}</Text>}
                description={
                  <Space direction="vertical">
                    <Text>Количество: {item.quantity}</Text>
                    <Text>Цена за штуку: {item.priceAtTime} ₽</Text>
                    <Text strong>
                      Итого: {item.quantity * item.priceAtTime} ₽
                    </Text>
                  </Space>
                }
              />
            </List.Item>
          </Card>
        )}
      />

      <div className="flex justify-between items-center mt-6">
        <div className="flex items-center gap-2">
          <Text strong style={{ fontSize: "1.2rem" }}>
            Статус:
          </Text>
          <Tag
            color={statusInfo[currentOrder.status]?.color || "default"}
            className="text-lg px-3 py-1"
          >
            {statusInfo[currentOrder.status]?.label || "Неизвестно"}
          </Tag>
        </div>
        <div className="flex items-center gap-4">
          <Text strong style={{ fontSize: "1.2rem" }}>
            Общая сумма: {currentOrder.totalAmount} ₽
          </Text>
          {currentOrder.status === OrderStatus.Pending && (
            <Button type="primary" onClick={handlePayOrder}>
              Оплатить заказ
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
