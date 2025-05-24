import { OrderRequest } from "@/app/types/Order/OrderRequest";
import adminAPI from "./adminAPI";
import { OrderResponse } from "@/app/types/Order/OrderResponce";

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
  try{
    await adminAPI.delete(`/AdminOrder?id=${opderId}`)
  } catch (err) {
    console.error("Ошибка при удалении заказа", err);
    throw err;
  }
}