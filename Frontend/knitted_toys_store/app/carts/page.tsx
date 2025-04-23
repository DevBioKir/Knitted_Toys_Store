"use client";

import { useEffect, useState } from "react";
import {
  Button,
  Card,
  List,
  message,
  Typography,
  Popconfirm,
  Space,
  Spin,
  Avatar,
} from "antd";
import { updateCart } from "../services/carts";
import { CartRequest } from "../types/Cart/CartRequest";
import { CartItemsRequest } from "../types/CartItems/CartItemsRequest";
import { useCart } from "../context/CartProvider";

const { Title, Text } = Typography;

export default function CartPage() {
  const { cart, refreshCart, isLoading } = useCart();
  const [isUpdating, setIsUpdating] = useState(false);

  // Используем правильное поле из интерфейса CartResponce
  const cartItemsResponces = cart?.cartItemsResponces || [];

  // Отладочная информация
  console.log("Состояние корзины:", {
    cart,
    cartItemsResponces,
    hasItems:
      Array.isArray(cartItemsResponces) && cartItemsResponces.length > 0,
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
      cartItems: items,
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

  const handleChangeQuantity = (toyId: string, delta: number) => {
    if (!cart) return;
    const updatedItems: CartItemsRequest[] = cartItemsResponces.map((item) => ({
      id: item.id,
      cartId: item.cartId,
      toyId: item.toyId,
      quantity: Math.max(1, item.quantity + (item.toyId === toyId ? delta : 0)),
      addedAt: item.addedAt,
      toyName: item.toyName,
      toyImageUrl: item.toyImageUrl,
    }));
    handleUpdateCart(updatedItems);
  };

  const handleRemoveItem = (toyId: string) => {
    if (!cart) return;
    const updatedItems: CartItemsRequest[] = cartItemsResponces
      .filter((item) => item.toyId !== toyId)
      .map((item) => ({
        id: item.id,
        cartId: item.cartId,
        toyId: item.toyId,
        quantity: item.quantity,
        addedAt: item.addedAt,
        toyName: item.toyName,
        toyImageUrl: item.toyImageUrl,
      }));
    handleUpdateCart(updatedItems);
  };

  const handleClearCart = () => {
    if (!cart) return;
    handleUpdateCart([]);
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
    Array.isArray(cartItemsResponces) && cartItemsResponces.length > 0;

  return (
    <div className="p-4 max-w-2xl mx-auto">
      <Title level={2}>Корзина</Title>
      {hasItems ? (
        <>
          <List
            itemLayout="horizontal"
            dataSource={cartItemsResponces}
            renderItem={(item) => (
              <Card className="mb-4">
                <List.Item
                  actions={[
                    <Popconfirm
                      title="Удалить товар?"
                      onConfirm={() => handleRemoveItem(item.toyId)}
                      okText="Да"
                      cancelText="Нет"
                      key="delete"
                    >
                      <Button danger>Удалить</Button>
                    </Popconfirm>,
                  ]}
                >
                  <List.Item.Meta
                    avatar={
                      <Avatar
                        src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${item.toyImageUrl}`}
                        alt={item.toyName}
                        size={300}
                        shape="square"
                        style={{
                          border: '1px solid #f0f0f0', // лёгкая рамка
                          objectFit: 'cover',
                        }}
                      />
                    }
                    title={<Text strong>{item.toyName}</Text>}
                    description={
                      <Space direction="vertical">
                        <Text>Количество: {item.quantity}</Text>
                        <Space>
                          <Button
                            onClick={() => handleChangeQuantity(item.toyId, -1)}
                          >
                            -
                          </Button>
                          <Text>{item.quantity}</Text>
                          <Button
                            onClick={() => handleChangeQuantity(item.toyId, 1)}
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
            <Popconfirm
              title="Оформить заказ"
              onConfirm={handleClearCart}
              okText="Да"
              cancelText="Нет"
            >
              <Button danger>Оформить заказ</Button>
            </Popconfirm>
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
