"use client";

const DEBUG = true;

function logDebug(...args: any[]) {
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import React, { createContext, useContext, useEffect, useRef, useState } from "react";
import { getCurrentCart } from "../services/carts";
import { CartResponce } from "../types/Cart/CartResponce";

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
  cart: CartResponce | null;
  refreshCart: () => Promise<void>;
  setCart: (cart: CartResponce | null) => void;
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
  const [cart, setCartState] = useState<CartResponce | null>(null);
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
        
        // Если есть cartItems, но нет cartItemsResponces, копируем данные
        if (!normalizedData.cartItemsResponces && normalizedData.cartItems) {
          logDebug("Копируем cartItems в cartItemsResponces");
          normalizedData.cartItemsResponces = normalizedData.cartItems;
        }
        
        // Проверяем, что cartItemsResponces - это массив
        if (!Array.isArray(normalizedData.cartItemsResponces)) {
          logDebug("cartItemsResponces не является массивом, устанавливаем пустой массив");
          normalizedData.cartItemsResponces = [];
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