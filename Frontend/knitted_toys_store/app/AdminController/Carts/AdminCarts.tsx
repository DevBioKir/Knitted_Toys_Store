"use client"

import { Cart } from "@/app/Models/Cart";
import { deleteCart, getAllCarts } from "@/app/services/carts";
import { Button, message } from "antd";
import { useEffect, useState } from "react"

export default function AdminCartsPage() {
    const [carts, setCarts] = useState<Cart[]>([]);
    const [loading, setLoading] = useState(true);
    //const [editingCart, setEditingCart] = useState<Cart | null>(null);
    //const [removedCart, setRemovedCart] = useState<Cart | null>(null);

    const fetchCart = async () => {
        try{
            const data = await getAllCarts();
            setCarts(data);
        } catch(err) {
            console.error(err);
            message.error("Произошла ошибка при загрузке корзин");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCart();
    }, [])

    const handleDelete = async (id: string) => {
        try{
            await deleteCart(id);
            message.success("Корзина удалена");
            fetchCart();
        } catch(err) {
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
                    <p><strong>{cart.id}</strong></p>
                    <p>Дата и время создания: {(cart.createAt).toString()}</p>
                    <p>Дата и время последнего изменения: {(cart.lastUpdate).toString()}</p>
                    <p>Общая сумма корзины: {cart.totalAmount}р</p>
                    <p>Состав корзины: {(cart.cartItems)}</p>

                    {/* Кнопки */}
                    {/* <Button
                      onClick={() => setEditingCart(cart)}
                      style={{ marginRight: 8 }}
                    >
                      Редактировать
                    </Button> */}
                    <Button onClick={() => handleDelete(cart.id!)}>
                      Удалить
                    </Button>
                  </div>
                </div>
              ))}
            </>
          )}
    
          {/* {editingToy && (
            <UpdateToyForm
              toy={editingToy}
              onSuccess={() => {
                setEditingToy(null);
                fetchToys();
              }}
            />
          )} */}
        </div>
      );
    }