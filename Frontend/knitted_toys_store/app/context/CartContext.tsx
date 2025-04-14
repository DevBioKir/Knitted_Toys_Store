"use client";
const DEBUG = true;

function logDebug(...args: any[]){
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import React, { createContext, useContext, useEffect, useState, useRef } from "react";
import { CartResponce } from "../types/Cart/CartResponce";
import { getCurrentCart } from "../services/carts";

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

// Глобальная переменная для отслеживания инициализации
let isInitializing = false;
let cartInitialized = false;

export const CartProvider = ({ children }: { children: React.ReactNode }) => {
  const [cart, setCartState] = useState<CartResponce | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const cartIdRef = useRef<string | null>(null);

  const refreshCart = async () => {
    // Если корзина уже загружается, не делаем ничего
    if (isLoading) return;
    
    try {
      setIsLoading(true);
      const data = await getCurrentCart();
      
      // Проверяем, не изменился ли ID корзины
      if (cartIdRef.current && cartIdRef.current !== data.id) {
        console.log(`Обнаружена новая корзина: ${data.id} (была: ${cartIdRef.current})`);
      }
      
      cartIdRef.current = data.id;
      setCartState(data);
    } catch (error) {
      console.error("Ошибка при получении корзины", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    // Проверяем, была ли корзина уже инициализирована
    if (cartInitialized) {
      console.log("Корзина уже инициализирована, пропускаем инициализацию");
      return;
    }
    
    // Проверяем, идет ли уже процесс инициализации
    if (isInitializing) {
      console.log("Инициализация уже идет, пропускаем");
      return;
    }
    
    isInitializing = true;
    
    const initCart = async () => {
      try {
        setIsLoading(true);
        const data = await getCurrentCart();
        cartIdRef.current = data.id;
        setCartState(data);
        cartInitialized = true;
      } catch (error) {
        console.error("Ошибка при получении корзины", error);
      } finally {
        setIsLoading(false);
        isInitializing = false;
      }
    };
    
    initCart();
  }, []);

  return (
    <CartContext.Provider value={{ cart, refreshCart, setCart: setCartState, isLoading }}>
      {children}
    </CartContext.Provider>
  );
};