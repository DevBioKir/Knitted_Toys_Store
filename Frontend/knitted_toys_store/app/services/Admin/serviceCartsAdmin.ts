const DEBUG = true;

function logDebug(...args: any[]) {
  if (DEBUG) {
    console.log(`[CartContext]`, ...args);
  }
}

import { CartResponse } from "@/app/types/Cart/CartResponse";
import { CartRequest } from "@/app/types/Cart/CartRequest";
import adminAPI from "./adminAPI";

export const getAllCartsAdmin = async (): Promise<CartResponse[]> => {
  try {
    const response = await adminAPI.get("/AdminCart/GetAllCartsAsyn");
    return response.data;
  } catch (err) {
    console.error("Ошибка при поиске всех корзин", err);
    throw err;
  }
};

export const getCartByIdAdmin = async (
  cartId: string
): Promise<CartResponse> => {
  try {
    const response = await adminAPI.get(`/AdminCart/${cartId}`);
    return response.data;
  } catch (err) {
    console.error("Ошибка при поиске корзины с ID", err);
    throw err;
  }
};

// Переменная для кэширования ID корзины
let cachedCartId: string | null = null;

export const getCurrentCartAdmin = async (): Promise<CartResponse> => {
  try {
    // Проверяем, есть ли ID корзины в cookie
    const cookies = document.cookie.split("; ");
    const cartCookie = cookies.find(
      (c) => c.startsWith("cart_id=") || c.startsWith("cartId=")
    );
    const cartIdFromCookie = cartCookie ? cartCookie.split("=")[1] : null;

    // Если ID корзины в cookie совпадает с кэшированным, выводим лог
    if (cartIdFromCookie && cachedCartId && cartIdFromCookie === cachedCartId) {
      logDebug(`Используем существующую корзину: ${cachedCartId}`);
    }

    // Обновляем кэшированный ID
    if (cartIdFromCookie) {
      cachedCartId = cartIdFromCookie;
    }

    const response = await adminAPI.get<CartResponse>("/AdminCart/Current", {
      headers: {
        "Cache-Control": "no-store",
      },
      params: {
        t: Date.now(), // обходим кеширование через URL
      },
    });

    // Получаем сырые данные
    const rawData = response.data;
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
};

//Возможно стоит сделать через админ http запрос
export const createCart = async (cartRequest: CartRequest) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminCart/CreateCart`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(cartRequest),
    }
  );

  if (!response.ok) {
    throw new Error("Не удалось создать корзину");
  }
};

export const updateCartAdmin = async (
  cartId: string,
  cartRequest: CartRequest
): Promise<CartResponse> => {
  try {
    const response = await adminAPI.put(
      `/AdminCart/UpdateCartAsync?cartId=${cartId}`,
      cartRequest
    );
    return response.data;
  } catch (err) {
    console.error("Ошибка при измении корзины", err);
    throw err;
  }
};

export const addToCart = async (
  cartId: string,
  toyId: string,
  quantity: number
) => {
  try {
    await adminAPI.post(
      `/AdminCart/AddToys?cartId=${cartId}&toyId=${toyId}&quantity=${quantity}`
    );
  } catch (err) {
    console.error("Ошибка при добавлении игрушки", err);
    throw err;
  }
};

export const reduceQuantityItem = async (cartId: string, toyId: string) => {
  try {
    await adminAPI.delete(
      `/AdminCart/ReduceQuantityItemAsync?cartId=${cartId}&toyId=${toyId}`
    );
  } catch (err) {
    console.error("Ошибка при удалении игрушки", err);
    throw err;
  }
};

export const deleteCartAdmin = async (cartId: string) => {
  try {
    await adminAPI.delete(`/AdminCart/DeleteCartAsync?id=${cartId}`);
  } catch (err) {
    console.error("Ошибка при удалении игрушки", err);
    throw err;
  }
};

export const removeFromCart = async (cartId: string, toyId: string) => {
  try {
    await adminAPI.delete(
      `/AdminCart/RemoveItemFromCart?cartId=${cartId}&toyId=${toyId}`
    );
  } catch (err) {
    console.error("Ошибка при удалении товара из корзины", err);
    throw err;
  }
};
