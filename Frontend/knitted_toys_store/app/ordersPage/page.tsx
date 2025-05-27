"use client";

import { useOrder } from "@/app/context/OrderProvider";
import { Button, Card, List, Space, Typography, Spin } from "antd";
import { useRouter } from "next/navigation";

const { Title, Text } = Typography;

export default function OrderPage() {
  const {
    selectedOrder,
    order,
    isLoading,
    refreshOrders,
  } = useOrder();

  const router = useRouter();

  const currentOrder = selectedOrder || order;

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (!currentOrder || currentOrder.orderItemsResponses?.length === 0) {
    return (
      <div className="text-center p-6">
        <Title level={3}>Заказ пуст</Title>
        <Button type="primary" onClick={() => router.push("/toysPage")}>
          Перейти в каталог
        </Button>
      </div>
    );
  }

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <Title level={2}>Текущий заказ</Title>
        <Space>
          <Button onClick={refreshOrders}>Обновить</Button>
          <Button onClick={() => router.back()}>Назад</Button>
        </Space>
      </div>

      <List
        dataSource={currentOrder.orderItemsResponses}
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

      <div className="flex justify-end mt-6">
        <Text strong style={{ fontSize: "1.2rem" }}>
          Общая сумма: {currentOrder.totalAmount} ₽
        </Text>
      </div>
    </div>
  );
}