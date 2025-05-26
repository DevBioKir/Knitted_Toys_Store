"use client";

import { useEffect, useState } from "react";
import { Order } from "../Models/Order";
import { Avatar, Button, Card, List, Modal, Space, Typography } from "antd";
import { useOrder } from "../context/OrderProvider";

const { Title, Text } = Typography;

export default function OrderPage() {
  const { selectedOrder, refreshOrders, isLoading } = useOrder();
  const [isUpdating, setIsUpdating] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Используем правильное поле из интерфейса CartResponse
  const OrderItemsResponses = selectedOrder?.orderItemsResponses || [];

  // Отладочная информация
  console.log("Состояние корзины:", {
    selectedOrder,
    OrderItemsResponses,
    hasItems:
      Array.isArray(OrderItemsResponses) && OrderItemsResponses.length > 0,
    isLoading,
  });

  // Принудительное обновление при монтировании
  useEffect(() => {
    refreshOrders();
  }, []);

  // const handleAddExistingItem = async (toyId: string) => {
  //   if (!cart) {
  //     message.error("Корзина не найдена");
  //     return;
  //   }
  //   try {
  //     await addToCart(cart.id, toyId, 1); // по умолчанию quantity = 1
  //     refreshCart();
  //     message.success("Товар добавлен в корзину");
  //   } catch (error) {
  //     console.error("Ошибка при добавлении в корзину:", error);
  //     message.error("Не удалось добавить товар в корзину");
  //   }
  // };

  // const handleReduceQuantityItem = async (toyId: string) => {
  //   if (!cart) {
  //     message.error("ID корзины не найден");
  //     return;
  //   }

  //   try {
  //     await reduceQuantityItem(cart.id, toyId);
  //     refreshCart();
  //     message.success("Количество товара уменьшено");
  //   } catch (error) {
  //     console.error("Ошибка при уменьшении количества товара:", error);
  //     message.error("Не удалось уменьшить количество товара");
  //   }
  // };

  // if (isLoading || isUpdating) {
  //   return (
  //     <div className="flex justify-center items-center h-64">
  //       <Spin size="large" />
  //     </div>
  //   );
  // }

  // Улучшенная проверка наличия товаров
  const hasItems =
    Array.isArray(OrderItemsResponses) && OrderItemsResponses.length > 0;

  return (
    <div className="p-4 max-w-2xl mx-auto">
      <Title level={2}>Мои заказы</Title>
      {hasItems ? (
        <>
          <List
            itemLayout="horizontal"
            dataSource={OrderItemsResponses}
            renderItem={(item) => (
              <Card className="mb-4">
                <List.Item>
                  <List.Item.Meta
                    avatar={
                      <Avatar
                        src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${item.toyImageUrl}`}
                        alt={item.toyName}
                        size={300}
                        shape="square"
                        style={{
                          border: "1px solid #f0f0f0", // лёгкая рамка
                          objectFit: "cover",
                        }}
                      />
                    }
                    title={<Text strong>{item.toyName}</Text>}
                    description={
                      <Space direction="vertical">
                        <Text>Количество: {item.quantity}</Text>
                        <Space>
                          <Button
                            onClick={() => handleReduceQuantityItem(item.toyId)}
                          >
                            -
                          </Button>
                          <Text>{item.quantity}</Text>
                          <Button
                            onClick={() => handleAddExistingItem(item.toyId)}
                          >
                            +
                          </Button>
                        </Space>
                      </Space>
                    }
                  />
                </List.Item>
              </Card>
            )}
          />
          <div className="flex justify-between items-center mt-4">
            <Text strong>Итого: {selectedOrder?.totalAmount} ₽</Text>
            <Button type="primary" onClick={() => setIsModalOpen(true)}>
              Оформить заказ
            </Button>
          </div>
        </>
      ) : (
        <div>
          <Text>У вас нет заказов.</Text>
          <Button onClick={refreshOrders} className="ml-4">
            Обновить заказы
          </Button>
        </div>
      )}
    </div>
  );
}