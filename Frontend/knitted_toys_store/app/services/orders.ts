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

export const createOrder = async (orderRequest: OrderRequest) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Order?surname=${orderRequest.surnameCustomer}
        &name=${orderRequest.nameCustomer}&phone=${orderRequest.phoneNumber}&email=${orderRequest.email}&deliveryAddress=${orderRequest.deliveryAddress}
        &deliveryNotes=${orderRequest.deliveryAddress}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(orderRequest),
    }
  );

  if (!response.ok) {
    throw new Error("Не удалось создать заказ");
  }
};
