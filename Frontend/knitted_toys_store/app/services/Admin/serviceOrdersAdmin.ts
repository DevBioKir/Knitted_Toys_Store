import { OrderRequest } from "@/app/types/Order/OrderRequest";
import adminAPI from "./adminAPI";
import { OrderResponse } from "@/app/types/Order/OrderResponce";
import { OrderStatus } from "@/app/Models/Order";

export const getOrderById = async (id: string): Promise<OrderResponse> => {
  try {
    const response = await adminAPI.get(`/AdminOrder/${id}`);
    return response.data;
  } catch (err) {
    console.error("Ошибка при получении заказа", err);
    throw err;
  }
};

export const getAllOrdersAdmin = async (): Promise<OrderResponse[]> => {
  try {
    const response = await adminAPI.get("/AdminOrder/GetAllOrdersAsync");
    return response.data;
  } catch (err) {
    console.error("Ошибка при получении заказов", err);
    throw err;
  }
};

export const createOrderAdmin = async (orderRequest: OrderRequest) => {
  try {
    await adminAPI.post(`/AdminOrder?surname=${orderRequest.surnameCustomer}
          &name=${orderRequest.nameCustomer}&phone=${orderRequest.phoneNumber}&email=${orderRequest.email}&deliveryAddress=${orderRequest.deliveryAddress}
          &deliveryNotes=${orderRequest.deliveryAddress}`);
  } catch (err) {
    console.error("Ошибка при создании заказа", err);
    throw err;
  }
};

export const deleteOrderAdmin = async (opderId: string) => {
  try {
    await adminAPI.delete(`/AdminOrder?id=${opderId}`);
  } catch (err) {
    console.error("Ошибка при удалении заказа", err);
    throw err;
  }
};

export const addToOrder = async (
  orderId: string,
  toyId: string,
  quantity: number
) => {
  try {
    await adminAPI.post(
      `/AdminOrder/AddToys?orderId=${orderId}&toyId=${toyId}&quantity=${quantity}`
    );
  } catch (err) {
    console.error("Ошибка при добавлении игрушки", err);
    throw err;
  }
};

export const reduceQuantityItem = async (orderId: string, toyId: string) => {
  try {
    await adminAPI.delete(
      `/AdminOrder/ReduceQuantityItemAsync?orderId=${orderId}&toyId=${toyId}`
    );
  } catch (err) {
    console.error("Ошибка при удалении игрушки", err);
    throw err;
  }
};

export const RemoveItemFromOrder = async (orderId: string, toyId: string) => {
  try {
    await adminAPI.delete(
      `/AdminOrder/RemoveItemFromCart?orderId=${orderId}&toyId=${toyId}`
    );
  } catch (err) {
    console.error("Ошибка при удалении товара из заказа", err);
    throw err;
  }
};

export const updateStatusOrder = async (id: string, newStatus: OrderStatus) => {
  try {
    await adminAPI.put(`/Order?orderId=${id}&newStatus=${newStatus}`);
  } catch (err) {
    console.error("Ошибка при изменении статуса заказа", err);
    throw err;
  }
};
