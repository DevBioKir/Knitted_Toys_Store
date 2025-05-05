"use client";

import { UpdateCartForm } from "@/app/components/Admin/Carts/UpdateCartForm";
import { useCart } from "@/app/context/CartProvider";
import { Cart } from "@/app/Models/Cart";
import {
  deleteCartAdmin,
  getAllCartsAdmin
} from "@/app/services/Admin/serviceCartsAdmin";
import { Button, List, message } from "antd";
import { useEffect, useState } from "react";

export default function AdminCartsPage() {
  const [carts, setCarts] = useState<Cart[]>([]);
  const [loading, setLoading] = useState(true);
  const { cart, refreshCart, isLoading } = useCart();
  const [editingCart, setEditingCart] = useState<Cart | null>(null);
  //const [removedCart, setRemovedCart] = useState<Cart | null>(null);

  const CartItemsResponses = cart?.cartItemsResponses || [];

  const fetchCart = async () => {
    try {
      const data = await getAllCartsAdmin();
      setCarts(data);
    } catch (err) {
      console.error(err);
      message.error("Произошла ошибка при загрузке корзин");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCart();
  }, []);

  const handleDelete = async (id: string) => {
    try {
      await deleteCartAdmin(id);
      message.success("Корзина удалена");
      fetchCart();
      message.success("страница с корзинами обновлена");
    } catch (err) {
      console.error(err);
      message.error("Не удалось удалить корзину");
    }
  };

  return (
    <div style={{ padding: "24px" }}>
      <h2>Корзины</h2>
      {loading ? (
        <p>Загрузка...</p>
      ) : (
        <>
          {carts.map((cart) => (
            <div
              key={cart.id}
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
                  <strong>{cart.id}</strong>
                </p>
                <p>Дата и время создания: {cart.createAt.toString()}</p>
                <p>
                  Дата и время последнего изменения:{" "}
                  {cart.lastUpdate.toString()}
                </p>
                <p>Общая сумма корзины: {cart.totalAmount}р</p>
                <List
                  itemLayout="horizontal"
                  dataSource={CartItemsResponses}
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
                      onClick={() => setEditingCart(cart)}
                      style={{ marginRight: 8 }}
                    >
                      Редактировать
                    </Button>
                <Button onClick={() => handleDelete(cart.id!)}>Удалить</Button>
              </div>
            </div>
          ))}
        </>
      )}

      {editingCart && (
        <UpdateCartForm
          cart={editingCart}
          onSuccess={() => {
            setEditingCart(null);
            fetchCart();
          }}
        />
      )}
    </div>
  );
}
