"use client";

import { Button, Form, Input, InputNumber, message, Select, Space } from "antd";
import { useEffect, useState } from "react";
import { CartRequest } from "@/app/types/Cart/CartRequest";
import { Cart } from "@/app/Models/Cart";
import { updateCartAdmin } from "@/app/services/Admin/serviceCartsAdmin";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { getAllToysAdmin } from "@/app/services/Admin/serviceToysAdmin";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";

interface Props {
  cart: Cart;
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
  
    form.setFieldsValue({
      createAt: cart.createAt,
      lastUpdate: cart.lastUpdate,
      totalAmount: cart.totalAmount,
      cartItemsRequest:
        cart.cartItems?.map((item) => ({
          toyId: item.toyId,
          quantity: item.quantity,
        })) || [],
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
          cart.cartItems?.length > 0 ? (
            cart.cartItems.map((item, index) => {
              const toyName =
                toys.find((toy) => toy.id === item.toyId)?.name ||
                "Неизвестная игрушка";
              return (
                <div key={index}>
                  🧸 <strong>{toyName}</strong> — {item.quantity} шт.
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
