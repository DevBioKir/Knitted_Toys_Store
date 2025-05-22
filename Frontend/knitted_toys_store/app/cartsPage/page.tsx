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
import { addToCart, reduceQuantityItem, updateCart } from "../services/carts";
import { CartRequest } from "../types/Cart/CartRequest";
import { CartItemsRequest } from "../types/CartItems/CartItemsRequest";
import { useCart } from "../context/CartProvider";
import OrderCreateForm from "../components/OrderCreateForm";

const { Title, Text } = Typography;

export default function CartPage() {
  const { cart, refreshCart, isLoading } = useCart();
  const [isUpdating, setIsUpdating] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);

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

  const handleUpdateCart = async (items: CartItemsRequest[]) => {
    if (!cart) return;
    setIsUpdating(true);

    const cartRequest: CartRequest = {
      id: cart.id,
      createAt: cart.createAt,
      lastUpdate: cart.lastUpdate,
      totalAmount: cart.totalAmount,
      rowVersion: cart.rowVersion,
      cartItemsRequest: items,
    };

    try {
      await updateCart(cart.id, cartRequest);
      await refreshCart();
      message.success("Корзина обновлена");
    } catch (error: any) {
      if (error.message.includes("409")) {
        message.warning("Корзина была изменена в другом месте. Обновляем...");
        refreshCart();
      } else {
        message.error("Ошибка при обновлении корзины");
        console.error(error);
      }
    } finally {
      setIsUpdating(false);
    }
  };

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

  // const handleClearCart = () => {
  //   if (!cart) return;
  //   handleUpdateCart([]);
  // };

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
    <div className="p-4 max-w-2xl mx-auto">
      <Title level={2}>Корзина</Title>
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
                  //handleClearCart(); // если хочешь очищать корзину после заказа
                }}
              />
            </Modal>
          </div>
        </>
      ) : (
        <div>
          <Text>Корзина пуста</Text>
          <Button onClick={refreshCart} className="ml-4">
            Обновить корзину
          </Button>
        </div>
      )}
    </div>
  );
}
