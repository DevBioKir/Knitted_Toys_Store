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

// export const getAllCartsAdmin = async (): Promise<CartResponse[]> => {
//     const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminCart/GetAllCartsAsyn`, {
//         method: "GET",
//         headers: {
//             "Content-Type": "application/json",
//         },
//         credentials: "include",
//     });

//     if (!response.ok) {
//         throw new Error("Failed to fetch carts");
//     }

//     const data: CartResponse[] = await response.json(); // Преобразуем ответ в массив объектов Cartresponse
//     return data; // Возвращаем данные
// };

export const getCartByIdAdmin = async (id: string): Promise<CartResponse> => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminCart/${id}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    }
  );

  if (!response.ok) {
    throw new Error(`Ошибка при получении корзины с ID ${id}`);
  }

  return response.json();
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

    // const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminCart/Current`, {
    //     method: "GET",
    //     headers: { "Content-Type": "application/json" },
    //     credentials: "include",
    //     cache: "no-store"
    // });

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
      `/AdminCart?cartId=${cartId}`,
      cartRequest
    );
    return response.data;
  } catch (err) {
    console.error("Ошибка при измении корзины", err);
    throw err;
  }
};

export const addToCart = async (
  idCart: string,
  idToy: string,
  quantity: number
) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminCart/AddToys?cartId=${idCart}&toyId=${idToy}&quantity=${quantity}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    }
  );

  if (!response.ok) {
    await response.text();
    throw new Error("Ошибка при добавлении игрушки в корзину");
  }

  return await response.json();
};

export const deleteCartAdmin = async (idCart: string) => {
  try {
    await adminAPI.delete(`/AdminCart/DeleteCartAsync?id=${idCart}`);
  } catch (err) {
    console.error("Ошибка при удалении игрушки", err);
    throw err;
  }
};

export const removeFromCart = async (idCart: string, idToy: string) => {
  try {
    await adminAPI.delete(
      `/AdminCart/RemoveItemFromCart?cartId=${idCart}&toyId=${idToy}`
    );
  } catch (err) {
    console.error("Ошибка при удалении товара из корзины", err);
    throw err;
  }
};
