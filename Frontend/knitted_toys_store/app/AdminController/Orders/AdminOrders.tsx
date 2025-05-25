"use client";

import { UpdateOrderForm } from "@/app/components/Admin/Orders/UpdateOrderForm";
import { Order } from "@/app/Models/Order";
import {
  deleteOrderAdmin,
  getAllOrdersAdmin,
} from "@/app/services/Admin/serviceOrdersAdmin";
import { OrderResponse } from "@/app/types/Order/OrderResponce";
import { Button, List, message } from "antd";
import { useEffect, useState } from "react";

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingOrder, setEditingOrder] = useState<OrderResponse | null>(null);

  // const OrderItemsResponses = order?.orderItemsResponses || [];

  const fetchOrders = async () => {
    try {
      const data = await getAllOrdersAdmin();
      console.log("API вернуло заказы:", data);

      const updateOrders = data.map(order => ({
        ...order,
        orderItemsResponses: order.orderItemsResponses || []
      }));

      setOrders(updateOrders);
    } catch (err) {
      console.error(err);
      message.error("Ошибка при загрузке заказов");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const handleDelete = async (id: string) => {
    try {
      await deleteOrderAdmin(id);
      message.success("Заказ удалён");
      fetchOrders();
    } catch (err) {
      console.error(err);
      message.error("Не удалось удалить заказ");
    }
  };

  return (
    <div style={{ padding: "24px" }}>
      <h2>Заказы</h2>
      {loading ? (
        <p>Загрузка...</p>
      ) : (
        <>
          {orders.map((order) => (
            <div
              key={order.id}
              style={{
                marginBottom: "20px",
                border: "1px solid #ddd",
                borderRadius: "8px",
                padding: "16px",
                display: "flex",
                gap: "16px",
                alignItems: "flex-start",
              }}
            >
              {/* Описание */}
              <div style={{ flexGrow: 1 }}>
                <p>
                  <strong>Id заказа: {order.id}</strong>
                </p>
                <p>Дата создания заказа: {order.orderDate}₽</p>
                <p>Сумма заказа: {order.totalAmount}</p>
                <p>Статус заказа: {order.status}</p>
                <p>Фамилия заказчика: {order.surnameCustomer}</p>
                <p>Имя заказчика: {order.nameCustomer}</p>
                <p>Телефон заказчика: {order.phoneNumber}</p>
                <p>Email заказчика: {order.email}</p>
                <p>Адрес доставки: {order.deliveryAddress}</p>
                <p>Примечание к заказу: {order.deliveryNotes}</p>

                <List
                  itemLayout="horizontal"
                  dataSource={order.orderItemsResponses || []} // Используем cartItemsResponses
                  renderItem={(item) => (
                    <List.Item>
                      <List.Item.Meta
                        avatar={
                          <img
                            src={`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}${item.toyImageUrl}`}
                            alt={item.toyName}
                            style={{
                              width: 40,
                              height: 40,
                              objectFit: "cover",
                              borderRadius: 4,
                            }}
                          />
                        }
                        title={item.toyName}
                        description={`Количество: ${item.quantity}`}
                      />
                    </List.Item>
                  )}
                />

                {/* Кнопки */}
                <Button
                  onClick={() => setEditingOrder(order)}
                  style={{ marginRight: 8 }}
                >
                  Редактировать
                </Button>
                <Button onClick={() => handleDelete(order.id!)}>Удалить</Button>
              </div>
            </div>
          ))}
        </>
      )}

      {editingOrder && (
        <UpdateOrderForm
          order={editingOrder}
          onSuccess={() => {
            setEditingOrder(null);
            fetchOrders(); // Перезагружаем корзины после редактирования
          }}
        />
      )}
    </div>
  );
}
