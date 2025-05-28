import { OrderStatus } from "../../Models/Order";
import { OrderItemsResponse } from "../OrderItems/OrderItemsResponse";


export interface OrderResponse{
    id?: string;
    orderDate?: Date;
    totalAmount?: number;
    status?: OrderStatus;
    surnameCustomer: string;
    nameCustomer: string;
    phoneNumber: string;
    email: string;
    deliveryAddress: string;
    deliveryNotes: string;
    orderItems?: OrderItemsResponse[];

    orderItemsResponse: OrderItemsResponse[];
}