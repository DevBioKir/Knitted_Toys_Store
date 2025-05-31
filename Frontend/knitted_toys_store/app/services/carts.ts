const DEBUG = true;

function logDebug(...args: any[]){
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import { CartRequest } from "../types/Cart/CartRequest";
import { CartResponse } from "../types/Cart/CartResponse";


export const getAllCarts = async (): Promise<CartResponse[]> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart/GetAllCartsAsyn`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch carts");
    }

    const data: CartResponse[] = await response.json(); // Преобразуем ответ в массив объектов Cartresponse
    return data; // Возвращаем данные
};

export const getCartById = async (id: string): Promise<CartResponse> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Ошибка при получении корзины с ID ${id}`);
    }

    return response.json();
};

// Переменная для кэширования ID корзины
let cachedCartId: string | null = null;

export const getCurrentCart = async(): Promise<CartResponse> => {
    try {
        // Проверяем, есть ли ID корзины в cookie
        const cookies = document.cookie.split('; ');
        const cartCookie = cookies.find(c => c.startsWith('cart_id=') || c.startsWith('cartId='));
        const cartIdFromCookie = cartCookie ? cartCookie.split('=')[1] : null;
        
        // Если ID корзины в cookie совпадает с кэшированным, выводим лог
        if (cartIdFromCookie && cachedCartId && cartIdFromCookie === cachedCartId) {
            logDebug(`Используем существующую корзину: ${cachedCartId}`);
        }
        
        // Обновляем кэшированный ID
        if (cartIdFromCookie) {
            cachedCartId = cartIdFromCookie;
        }
        
        const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart/Current`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            cache: "no-store"
        });
        
        if (!response.ok) {
            throw new Error("Не удалось получить текущую корзину");
        }
        
        // Получаем сырые данные
        const rawData = await response.json();
        logDebug("Сырые данные от API:", rawData);
        
        // Нормализуем данные
        let cartData: CartResponse;
        
        // Проверяем структуру данных
        if (Array.isArray(rawData)) {
            logDebug("API вернул массив, берем первый элемент");
            cartData = rawData[0];
        } else {
            logDebug("API вернул объект");
            cartData = rawData;
        }
        
        // Нормализуем поля
        if (!cartData.cartItemsResponses && cartData.cartItems) {
            logDebug("Копируем cartItems в CartItemsResponses");
            cartData.cartItemsResponses = cartData.cartItems;
        }
        
        return cartData;
    } catch (error) {
        console.error("Ошибка при получении корзины:", error);
        throw error;
    }
}

export const updateCart = async (cartId: string, cartRequest: CartRequest): Promise<CartResponse> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart?cartId=${cartId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(cartRequest),
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Ошибка при обновлении корзины: ${errorText}`);
    }

    return await response.json();
};

export const addToCart = async (cartId: string, idToy: string, quantity: number) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart/AddToys?cartId=${cartId}&toyId=${idToy}&quantity=${quantity}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    if (!response.ok) {
        await response.text();
        throw new Error("Ошибка при добавлении игрушки в корзину")
    }

    return await response.json();
};

export const reduceQuantityItem = async (cartId: string, toyId: string) => {
  try {
    return await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Cart/ReduceQuantityItemAsync?cartId=${cartId}&toyId=${toyId}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
  } catch (err) {
    console.error("Ошибка при удалении игрушки", err);
    throw err;
  }
};
