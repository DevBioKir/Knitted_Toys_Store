"use client";

import { OrderResponse } from "../types/Order/OrderResponce";
import React, {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import { getCurrentOrder } from "../services/orders";

const DEBUG = true;

function logDebug(...args: any[]) {
  if (DEBUG) {
    console.log(`[OrderContext]`, ...args);
  }
}

function waitForOrderCookie(timeoutMs = 2000): Promise<void> {
  const start = Date.now();
  return new Promise((resolve) => {
    const check = () => {
      const orderId = document.cookie
        .split("; ")
        .find(
          (row) => row.startsWith("orderId=") || row.startsWith("order_id=")
        );
      if (orderId || Date.now() - start > timeoutMs) {
        resolve();
      } else {
        setTimeout(check, 50);
      }
    };
    check();
  });
}

type OrderContextType = {
  order: OrderResponse | null;
  selectedOrder: OrderResponse | null;
  setSelectedOrder: (order: OrderResponse | null) => void;
  refreshOrders: () => Promise<void>;
  isLoading: boolean;
};

const OrderContext = createContext<OrderContextType>({
  order: null,
  selectedOrder: null,
  setSelectedOrder: () => {},
  refreshOrders: async () => {},
  isLoading: false,
});

export const useOrder = () => useContext(OrderContext);

export const OrderProvider = ({ children }: { children: React.ReactNode }) => {
  const [order, setOrder] = useState<OrderResponse | null>(null);
  const [selectedOrder, setSelectedOrder] = useState<OrderResponse | null>(
    null
  );
  const [isLoading, setIsLoading] = useState(false);
  const hasInitialized = useRef(false);
  const refreshInProgress = useRef(false);

  const refreshOrders = async () => {
    if (refreshInProgress.current) return;
    refreshInProgress.current = true;
    setIsLoading(true);

    try {
      await waitForOrderCookie();
      //getAllOrders - тут пользовательский метод
      const data = await getCurrentOrder();
      logDebug("Получены данные корзины:", data);

      if (data) {
        // Нормализуем данные для фронтенда
        const normalizedData = { ...data };

        // Если есть cartItems, но нет CartItemsResponses, копируем данные
        if (!normalizedData.orderItemsResponse && normalizedData.orderItems) {
          logDebug("Копируем cartItems в CartItemsResponses");
          normalizedData.orderItemsResponse = normalizedData.orderItems;
        }

        // Проверяем, что orderItemsResponse - это массив
        if (!Array.isArray(normalizedData.orderItemsResponse)) {
          logDebug(
            "orderItemsResponse не является массивом, устанавливаем пустой массив"
          );
          normalizedData.orderItemsResponse = [];
        }

        setOrder(normalizedData);
        setSelectedOrder(normalizedData);
      }
    } catch (error) {
      console.error("Ошибка при получении заказа", error);
    } finally {
      setIsLoading(false);
      refreshInProgress.current = false;
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
