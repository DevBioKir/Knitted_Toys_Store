"use client";

import { useEffect, useState } from "react";
import {
  Button,
  Card,
  List,
  message,
  Typography,
  Space,
  Spin,
  Avatar,
  Modal,
} from "antd";
import { addToCart, reduceQuantityItem } from "../services/carts";
import { useCart } from "../context/CartProvider";
import OrderCreateForm from "../components/OrderCreateForm";
import { useRouter } from "next/navigation";

const { Title, Text } = Typography;

export default function CartPage() {
  const { cart, refreshCart, isLoading } = useCart();
  const [isUpdating, setIsUpdating] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const router = useRouter();

  // Используем правильное поле из интерфейса CartResponse
  const CartItemsResponses = cart?.cartItemsResponses || [];

  // Отладочная информация
  console.log("Состояние корзины:", {
    cart,
    CartItemsResponses,
    hasItems:
      Array.isArray(CartItemsResponses) && CartItemsResponses.length > 0,
    isLoading,
  });

  // Принудительное обновление при монтировании
  useEffect(() => {
    refreshCart();
  }, []);

  const handleAddExistingItem = async (toyId: string) => {
    if (!cart) {
      message.error("Корзина не найдена");
      return;
    }
    try {
      await addToCart(cart.id, toyId, 1); // по умолчанию quantity = 1
      refreshCart();
      message.success("Товар добавлен в корзину");
    } catch (error) {
      console.error("Ошибка при добавлении в корзину:", error);
      message.error("Не удалось добавить товар в корзину");
    }
  };

  const handleReduceQuantityItem = async (toyId: string) => {
    if (!cart) {
      message.error("ID корзины не найден");
      return;
    }

    try {
      await reduceQuantityItem(cart.id, toyId);
      refreshCart();
      message.success("Количество товара уменьшено");
    } catch (error) {
      console.error("Ошибка при уменьшении количества товара:", error);
      message.error("Не удалось уменьшить количество товара");
    }
  };

  if (isLoading || isUpdating) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  // Улучшенная проверка наличия товаров
  const hasItems =
    Array.isArray(CartItemsResponses) && CartItemsResponses.length > 0;

  return (
    <div className="text-center p-6">
      {hasItems ? (
        <>
          <List
            itemLayout="horizontal"
            dataSource={CartItemsResponses}
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
            <Text strong>Итого: {cart?.totalAmount} ₽</Text>
            <Button type="primary" onClick={() => setIsModalOpen(true)}>
              Оформить заказ
            </Button>

            <Modal
              title="Оформление заказа"
              open={isModalOpen}
              onCancel={() => setIsModalOpen(false)}
              footer={null}
            >
              <OrderCreateForm
                onOrderCreated={() => {
                  message.success("Заказ создан");
                  setIsModalOpen(false);
                  refreshCart();
                }}
              />
            </Modal>
          </div>
        </>
      ) : (
        <div className="text-center p-6">
          <Title level={3}>Корзина пуста</Title>
          <p className="mb-4 text-gray-500">
            Возможно, вы уже оформили заказ или не добавили товары
          </p>
          <div className="flex flex-col items-center">
            <div className="flex gap-4">
              <Button onClick={refreshCart}>Обновить</Button>
              <Button type="primary" onClick={() => router.push("/toysPage")}>
                Перейти в каталог
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
