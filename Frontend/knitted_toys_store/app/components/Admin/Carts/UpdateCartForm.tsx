"use client";

import { Button, Form, Input, InputNumber, message, Select, Space } from "antd";
import { useEffect, useState } from "react";
import { CartRequest } from "@/app/types/Cart/CartRequest";
import {
  reduceQuantityItem,
  removeFromCart,
  updateCartAdmin,
} from "@/app/services/Admin/serviceCartsAdmin";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { getAllToysAdmin } from "@/app/services/Admin/serviceToysAdmin";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";
import { CartResponse } from "@/app/types/Cart/CartResponse";
import { addToCart } from "@/app/services/carts";

interface Props {
  cart: CartResponse;
  onSuccess: () => void;
}

export const UpdateCartForm = ({ cart, onSuccess }: Props) => {
  const [form] = Form.useForm<CartRequest>();
  const [toys, setToys] = useState<ToyResponse[]>();

  useEffect(() => {
    console.log("Получена корзина:", cart);
    console.log("Состав корзины (cartItems):", cart.cartItems);

    getAllToysAdmin()
      .then((toys) => {
        const sorted = [...toys].sort((a, b) => a.name.localeCompare(b.name));
        setToys(sorted);
      })
      .catch(() => message.error("Не удалось загрузить игрушки"));

    if (!cart || !cart.id) return;

    form.setFieldsValue({
      createAt: cart.createAt,
      lastUpdate: cart.lastUpdate,
      totalAmount: cart.totalAmount,
      cartItemsRequest: [],
      rowVersion: cart.rowVersion,
    });
  }, [cart, form]);

  const handleSubmit = async (cartRequest: CartRequest) => {
    try {
      if (!cart.id) {
        message.error("Такого id корзины нет");
        return;
      }
      console.log("Отправка данных:", cartRequest);
      await updateCartAdmin(cart.id, cartRequest);
      message.success("Корзина обновлена");
      onSuccess();
    } catch (err) {
      console.error(err);
      message.error("Ошибка при обновлении корзины");
    }
  };

  const handleAddExistingItem = async (toyId: string) => {
    if (!cart) {
      message.error("Корзина не найдена");
      return;
    }
    try {
      await addToCart(cart.id, toyId, 1); // по умолчанию quantity = 1
      message.success("Товар добавлен в корзину");
    } catch (error) {
      console.error("Ошибка при добавлении в корзину:", error);
      message.error("Не удалось добавить товар в корзину");
    }
  };

  const handleReduceQuantityItem = async (toyId: string) => {
    if (!cart.id) {
      message.error("ID корзины не найден");
      return;
    }

    try {
      await reduceQuantityItem(cart.id, toyId);
      message.success("Количество товара уменьшено");
    } catch (error) {
      console.error("Ошибка при уменьшении количества товара:", error);
      message.error("Не удалось уменьшить количество товара");
    }
  };

  const handleRemoveExistingItem = async (toyId: string) => {
    if (!cart.id) {
      message.error("ID корзины не найден");
      return;
    }

    try {
      await removeFromCart(cart.id, toyId);
      const updatedItems =
        cart.cartItemsResponses?.filter((item) => item.toyId !== toyId) || [];
      cart.cartItemsResponses = updatedItems;

      message.success("Товар удалён из корзины");
    } catch {
      message.error("Не удалось удалить товар");
    }
  };

  return (
    <Form layout="vertical" form={form} onFinish={handleSubmit}>
      <Form.Item name="createAt" label="Дата создания корзины">
        <Input />
      </Form.Item>

      <Form.Item name="lastUpdate" label="Дата изменения корзины">
        <Input />
      </Form.Item>

      <Form.Item name="totalAmount" label="Общая сумма">
        <InputNumber style={{ width: "100%" }} readOnly />
      </Form.Item>

      <Form.Item name="rowVersion" label="rowVersion">
        <Input readOnly />
      </Form.Item>

      {/* Превью текущих товаров в корзине */}
      <div style={{ marginBottom: "16px" }}>
        <h3>Текущие товары в корзине:</h3>

        {toys ? (
          cart.cartItemsResponses?.length > 0 ? (
            cart.cartItemsResponses.map((item, index) => {
              const toyName =
                toys.find((toy) => toy.id === item.toyId)?.name ||
                "Неизвестная игрушка";
              return (
                <div key={index}>
                  🧸 <strong>{toyName}</strong> — {item.quantity} шт.
                  <Button
                    size="small"
                    onClick={() => handleAddExistingItem(item.toyId)}
                  >
                    +
                  </Button>
                  <Button
                    size="small"
                    onClick={() => handleReduceQuantityItem(item.toyId)}
                  >
                    −
                  </Button>
                  <Button
                    type="link"
                    danger
                    onClick={() => handleRemoveExistingItem(item.toyId)}
                  >
                    Удалить позицию товара
                  </Button>
                </div>
              );
            })
          ) : (
            <div>Корзина пуста</div>
          )
        ) : (
          <div>Загрузка игрушек...</div>
        )}
      </div>

      {/* Редактируемые поля */}
      <Form.List name="cartItemsRequest">
        {(fields, { add, remove }) => (
          <>
            {fields.map(({ key, name, ...restField }) => (
              <Space
                key={key}
                align="baseline"
                style={{ display: "flex", marginBottom: 8 }}
              >
                <Form.Item
                  {...restField}
                  name={[name, "toyId"]}
                  rules={[{ required: true, message: "Выберите игрушку" }]}
                >
                  <Select placeholder="Выберите игрушку" style={{ width: 200 }}>
                    {toys?.map((toy) => (
                      <Select.Option key={toy.id} value={toy.id}>
                        {toy.name}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>

                <Form.Item
                  {...restField}
                  name={[name, "quantity"]}
                  rules={[{ required: true, message: "Укажите количество" }]}
                >
                  <InputNumber placeholder="Введите количество" min={1} />
                </Form.Item>

                <MinusCircleOutlined onClick={() => remove(name)} />
              </Space>
            ))}
            <Form.Item>
              <Button
                type="dashed"
                onClick={() => add()}
                icon={<PlusOutlined />}
                block
              >
                Добавить позицию
              </Button>
            </Form.Item>
          </>
        )}
      </Form.List>

      <Form.Item>
        <Button type="primary" htmlType="submit" block>
          Сохранить
        </Button>
      </Form.Item>
    </Form>
  );
};
