"use client";

import {
  Button,
  Form,
  Input,
  InputNumber,
  message,
  Select,
  Divider,
  Typography,
} from "antd";
import { useEffect, useState } from "react";
import { CartRequest } from "@/app/types/Cart/CartRequest";
import {
  addToCart,
  reduceQuantityItem,
  removeFromCart,
} from "@/app/services/Admin/serviceCartsAdmin";
import { getAllToysAdmin } from "@/app/services/Admin/serviceToysAdmin";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";
import { CartResponse } from "@/app/types/Cart/CartResponse";

interface Props {
  cart: CartResponse;
  onSuccess: () => void;
}

export const UpdateCartForm = ({ cart, onSuccess }: Props) => {
  const [form] = Form.useForm<CartRequest>();
  const [toys, setToys] = useState<ToyResponse[]>();
  const [selectedToy, setSelectedToy] = useState<string | null>(null);
  const [quantity, setQuantity] = useState<number>(1);

  useEffect(() => {
    getAllToysAdmin()
      .then((toys) =>
        setToys([...toys].sort((a, b) => a.name.localeCompare(b.name)))
      )
      .catch(() => message.error("Не удалось загрузить игрушки"));

    if (cart) {
      form.setFieldsValue({
        createAt: cart.createAt,
        lastUpdate: cart.lastUpdate,
        totalAmount: cart.totalAmount,
        rowVersion: cart.rowVersion,
      });
    }
  }, [cart, form]);

  const handleAddItemToCart = async (toyId: string, quantity: number) => {
    if (!cart) return;
    try {
      if (!cart.id) {
        console.error("Cart ID отсутствует");
        return;
      }
      await addToCart(cart.id, toyId, quantity);
      message.success(`Игрушка ${toyId} успешно добавлена`);
      onSuccess();
    } catch (err) {
      console.error(err);
    }
  };

  const handleReduceQuantityItem = async (toyId: string) => {
    if (!cart) return;
    try {
      if (!cart.id) {
        console.error("Cart ID отсутствует");
        return;
      }
      await reduceQuantityItem(cart.id, toyId);
      message.success(`Количество игрушек ${toyId} успешно уменьшено`);
      onSuccess();
    } catch (err) {
      console.error(err);
    }
  };

  const handleRemoveItemFromCart = async (toyId: string) => {
    if (!cart) return;
    if (!cart.id) {
      console.error("Cart ID отсутствует");
      return;
    }
    await removeFromCart(cart.id, toyId);
    onSuccess();
  };

  const handleAddToy = async () => {
    if (selectedToy && quantity > 0) {
      await handleAddItemToCart(selectedToy, quantity);
      setSelectedToy(null);
      setQuantity(1);
    }
  };

  return (
    <Form layout="vertical" form={form}>
      <Typography.Title level={3}>Характеристики корзины</Typography.Title>
      <Form.Item name="createAt" label="Дата создания корзины">
        <Input readOnly />
      </Form.Item>
      <Form.Item name="lastUpdate" label="Дата изменения корзины">
        <Input readOnly />
      </Form.Item>
      <Form.Item name="totalAmount" label="Общая сумма">
        <InputNumber style={{ width: "100%" }} readOnly />
      </Form.Item>
      <Form.Item name="rowVersion" label="Версия строки">
        <Input readOnly />
      </Form.Item>

      <Divider />

      <Typography.Title level={3}>Текущие товары в корзине</Typography.Title>
      {cart.cartItemsResponses?.length ? (
        cart.cartItemsResponses.map((item) => (
          <div
            key={item.toyId}
            style={{ display: "flex", alignItems: "center", marginBottom: 8 }}
          >
            <span>
              {toys?.find((toy) => toy.id === item.toyId)?.name ||
                "Неизвестная игрушка"}{" "}
              — {item.quantity} шт.
            </span>
            <Button onClick={() => handleAddItemToCart(item.toyId, 1)}>
              +
            </Button>
            <Button onClick={() => handleReduceQuantityItem(item.toyId)}>
              -
            </Button>
            <Button
              type="link"
              danger
              onClick={() => handleRemoveItemFromCart(item.toyId)}
            >
              Удалить
            </Button>
          </div>
        ))
      ) : (
        <p>Корзина пуста</p>
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
          Добавить в корзину
        </Button>
      </Form.Item>
    </Form>
  );
};
