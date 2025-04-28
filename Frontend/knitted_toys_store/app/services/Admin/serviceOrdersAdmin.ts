import { OrderRequest } from "@/app/types/Order/OrderRequest";


export const getOrderById = async (id: string) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminOrder/${id}`,
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

export const getAllOrders = async () => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminOrder`,
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

export const createOrder = async (orderRequest: OrderRequest) => {
  const response = await fetch(
    `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminOrder?surname=${orderRequest.surnameCustomer}
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
