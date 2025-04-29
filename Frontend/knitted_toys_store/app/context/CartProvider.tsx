"use client";

const DEBUG = true;

function logDebug(...args: any[]) {
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import React, { createContext, useContext, useEffect, useRef, useState } from "react";
import { getCurrentCart } from "../services/carts";
import { CartResponse } from "../types/Cart/CartResponse";

function waitForCartCookie(timeoutMs = 2000): Promise<void> {
  const start = Date.now();
  return new Promise((resolve) => {
    const check = () => {
      const cartId = document.cookie
        .split("; ")
        .find((row) => row.startsWith("cartId=") || row.startsWith("cart_id="));
      if (cartId || Date.now() - start > timeoutMs) {
        resolve();
      } else {
        setTimeout(check, 50);
      }
    };
    check();
  });
}

type CartContextType = {
  cart: CartResponse | null;
  refreshCart: () => Promise<void>;
  setCart: (cart: CartResponse | null) => void;
  isLoading: boolean;
};

const CartContext = createContext<CartContextType>({
  cart: null,
  refreshCart: async () => {},
  setCart: () => {},
  isLoading: false,
});

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }: { children: React.ReactNode }) => {
  const [cart, setCartState] = useState<CartResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const hasInitialized = useRef(false);
  const refreshInProgress = useRef(false);

  const refreshCart = async () => {
    if (refreshInProgress.current) return;
    refreshInProgress.current = true;
    setIsLoading(true);
    
    try {
      await waitForCartCookie();
      const data = await getCurrentCart();
      
      logDebug("Получены данные корзины:", data);
      
      if (data) {
        // Нормализуем данные для фронтенда
        const normalizedData = { ...data };
        
        // Если есть cartItems, но нет CartItemsResponses, копируем данные
        if (!normalizedData.cartItemsResponses && normalizedData.cartItems) {
          logDebug("Копируем cartItems в CartItemsResponses");
          normalizedData.cartItemsResponses = normalizedData.cartItems;
        }
        
        // Проверяем, что CartItemsResponses - это массив
        if (!Array.isArray(normalizedData.cartItemsResponses)) {
          logDebug("CartItemsResponses не является массивом, устанавливаем пустой массив");
          normalizedData.cartItemsResponses = [];
        }
        
        setCartState(normalizedData);
      }
    } catch (error) {
      console.error("Ошибка при получении корзины", error);
    } finally {
      setIsLoading(false);
      refreshInProgress.current = false;
    }
  };

  useEffect(() => {
    if (hasInitialized.current) return;
    hasInitialized.current = true;
    
    refreshCart();
  }, []);

  return (
    <CartContext.Provider value={{ cart, refreshCart, setCart: setCartState, isLoading }}>
      {children}
    </CartContext.Provider>
  );
};