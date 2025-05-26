"use client";

import { OrderResponse } from "../types/Order/OrderResponce";
import { createContext, useContext, useEffect, useRef, useState } from "react";
import { getAllOrders } from "../services/orders";

type OrderContextType = {
  order: OrderResponse[];
  selectedOrder: OrderResponse | null;
  setSelectedOrder: (order: OrderResponse | null) => void;
  refreshOrders: () => Promise<void>;
  isLoading: boolean;
};

const OrderContext = createContext<OrderContextType>({
  order: [],
  selectedOrder: null,
  setSelectedOrder: () => {},
  refreshOrders: async () => {},
  isLoading: false,
});

export const useOrder = () => useContext(OrderContext);

export const OrderProvider = ({ children }: { children: React.ReactNode }) => {
  const [order, setOrders] = useState<OrderResponse[]>([]);
  const [selectedOrder, setSelectedOrder] = useState<OrderResponse | null>(
    null
  );
  const [isLoading, setIsLoading] = useState(false);
  const hasInitialized = useRef(false);

  const refreshOrders = async () => {
    setIsLoading(true);
    try {
      //getAllOrders - тут пользовательский метод
      const data = await getAllOrders();
      setOrders(data ?? []);
    } catch (error) {
      console.error("Ошибка при получении заказов", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (hasInitialized.current) return;
    hasInitialized.current = true;
    refreshOrders();
  }, []);

  return (
    <OrderContext.Provider
      value={{
        order,
        selectedOrder,
        setSelectedOrder,
        refreshOrders,
        isLoading,
      }}
    >
      {children}
    </OrderContext.Provider>
  );
};
