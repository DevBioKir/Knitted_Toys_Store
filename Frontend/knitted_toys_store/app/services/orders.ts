import { OrderStatus } from "../Models/Order";
import { OrderRequest } from "../types/Order/OrderRequest";
import { OrderResponse } from "../types/Order/OrderResponce";

export const getOrderById = async (id: string) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order/${id}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    }
  );
  if (!response.ok) {
    throw new Error("Не удалось найти заказы");
  }
  response.json();
};

export const getAllOrders = async (): Promise<OrderResponse[]> => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    }
  );
  if (!response.ok) {
    throw new Error("Не удалось найти заказы");
  }
  const data = await response.json();
  return data;
};

// Переменная для кэширования ID заказа
let cachedOrderId: string | null = null;

export const getCurrentOrder = async (): Promise<OrderResponse | null> => {
  try {
    // Проверяем, есть ли ID заказа в cookie
    const cookies = document.cookie.split("; ");
    const orderCookie = cookies.find(
      (c) => c.startsWith("order_id=") || c.startsWith("orderId=")
    );
    const orderIdFromCookie = orderCookie ? orderCookie.split("=")[1] : null;

    // Если ID заказа в cookie совпадает с кэшированным, выводим лог
    if (
      orderIdFromCookie &&
      cachedOrderId &&
      orderIdFromCookie === cachedOrderId
    ) {
      logDebug(`Найдем существующий заказ: ${cachedOrderId}`);
    }

    // Обновляем кэшированный ID
    if (orderIdFromCookie) {
      cachedOrderId = orderIdFromCookie;
    }

    const response = await fetch(
      `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order/Current`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        cache: "no-store",
      }
    );

    if (response.status === 404) {
      logDebug("Текущий заказ не найден (404)");
      return null;
    }

    if (!response.ok) {
      throw new Error("Не удалось получить текущий заказ");
    }

    // Получаем сырые данные
    const rawData = await response.json();
    logDebug("Сырые данные от API:", rawData);

    // Нормализуем данные
    let orderData: OrderResponse;

    // Проверяем структуру данных
    if (Array.isArray(rawData)) {
      logDebug("API вернул массив, берем первый элемент");
      orderData = rawData[0];
    } else {
      logDebug("API вернул объект");
      orderData = rawData;
    }

    //Нормализуем поля
    if (!orderData.orderItemsResponse && orderData.orderItems) {
      logDebug("Копируем cartItems в CartItemsResponses");
      orderData.orderItemsResponse = orderData.orderItems;
    }

    return orderData;
  } catch (error) {
    console.error("Ошибка при получении заказа:", error);
    throw error;
  }
};

export const createOrder = async (
  orderRequest: OrderRequest
): Promise<
  | { success: true; data: any }
  | { success: false; warning: string }
  | { success: false; error: string }
> => {
  try {
    const response = await fetch(
      `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order?surname=${orderRequest.surnameCustomer}
          &name=${orderRequest.nameCustomer}&phone=${orderRequest.phoneNumber}&email=${orderRequest.email}&deliveryAddress=${orderRequest.deliveryAddress}
          &deliveryNotes=${orderRequest.deliveryNotes}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(orderRequest),
      }
    );

    if (response.ok) {
      const data = await response.json();
      return { success: true, data };
    }

    // Пытаемся считать JSON-ответ от сервера
    let body: any;
    try {
      body = await response.json();
    } catch {
      body = null;
    }

    // Сервер вернул предсказуемое предупреждение
    if (response.status === 400 && body?.message) {
      return { success: false, warning: body.message };
    }

    return {
      success: false,
      error: body?.message ?? "Неизвестная ошибка при создании заказа",
    };
  } catch (err) {
    return { success: false, error: "Ошибка соединения с сервером" };
  }
};


// export const createOrder = async (orderRequest: OrderRequest) => {
//   const response = await fetch(
//     `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order?surname=${orderRequest.surnameCustomer}
//         &name=${orderRequest.nameCustomer}&phone=${orderRequest.phoneNumber}&email=${orderRequest.email}&deliveryAddress=${orderRequest.deliveryAddress}
//         &deliveryNotes=${orderRequest.deliveryNotes}`,
//     {
//       method: "POST",
//       headers: {
//         "Content-Type": "application/json",
//       },
//       credentials: "include",
//       body: JSON.stringify(orderRequest),
//     }
//   );

//   if (!response.ok) {
//     throw new Error("Не удалось создать заказ");
//   }
// };

export const updateStatusOrder = async (id: string, newStatus: OrderStatus) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order?orderId=${id}&newStatus=${newStatus}`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    }
  );
  if (!response.ok) {
    throw new Error("Не удалось изменить статус заказа");
  }
  return response.json();
};

const DEBUG = true;

function logDebug(...args: any[]) {
  if (DEBUG) {
    console.log("[OrderService]", ...args);
  }
}
