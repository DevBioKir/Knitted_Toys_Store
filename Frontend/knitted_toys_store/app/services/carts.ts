const DEBUG = true;

function logDebug(...args: any[]){
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import { CartRequest } from "../types/Cart/CartRequest";
import { CartResponce } from "../types/Cart/CartResponce";


export const getAllCarts = async (): Promise<CartResponce[]> => {
    const response = await fetch(`http://localhost:5237/Cart`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error("Failed to fetch carts");
    }

    const data: CartResponce[] = await response.json(); // Преобразуем ответ в массив объектов CartResponce
    return data; // Возвращаем данные
};

export const getCartById = async (id: string): Promise<CartResponce> => {
    const response = await fetch(`http://localhost:5237/Cart/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error(`Ошибка при получении корзины с ID ${id}`);
    }

    return response.json();
};

// Переменная для кэширования ID корзины
let cachedCartId: string | null = null;

export const getCurrentCart = async(): Promise<CartResponce> => {
    try {
        // Проверяем, есть ли ID корзины в cookie
        const cookies = document.cookie.split('; ');
        const cartCookie = cookies.find(c => c.startsWith('cart_id='));
        const cartIdFromCookie = cartCookie ? cartCookie.split('=')[1] : null;
        
        // Если ID корзины в cookie совпадает с кэшированным, выводим лог
        if (cartIdFromCookie && cachedCartId && cartIdFromCookie === cachedCartId) {
            console.log(`Используем существующую корзину: ${cachedCartId}`);
        }
        
        // Обновляем кэшированный ID
        if (cartIdFromCookie) {
            cachedCartId = cartIdFromCookie;
        }
        
        const response = await fetch("http://localhost:5237/Cart/Current", {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            cache: "no-store"
        });
        
        if (!response.ok) {
            throw new Error("Не удалось получить текущую корзину");
        }
            
        const data: CartResponce = await response.json();
        
        // Обновляем кэшированный ID после получения ответа
        cachedCartId = data.id;
        
        return data;
    } catch (error) {
        console.error("Ошибка при получении корзины:", error);
        throw error;
    }
}

export const createCart = async (cartRequest: CartRequest) => {
    const response = await fetch ("http://localhost:5237/Cart/CreateCart", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(cartRequest),
    });

    if (!response.ok) {
        throw new Error("Не удалось создать корзину");
    }
};

export const updateCart = async (cartId: string, cartRequest: CartRequest): Promise<CartResponce> => {
    const response = await fetch(`http://localhost:5237/Cart?cartId=${cartId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(cartRequest),
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Ошибка при обновлении корзины: ${errorText}`);
    }

    return await response.json();
};