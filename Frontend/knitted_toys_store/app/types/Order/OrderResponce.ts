import { OrderStatus } from "../../Models/Order";
import { OrderItemsResponse } from "../OrderItems/OrderItemsResponse";


export interface OrderResponse{
    id?: string;
    odredDate?: Date;
    totalAmount?: number;
    status?: OrderStatus;
    surnameCustomer: string;
    nameCustomer: string;
    phoneNumber: string;
    email: string;
    deliveryAddress: string;
    deliveryNotes: string;
    orderItems?: OrderItemsResponse[];

    orderItemsResponses: OrderItemsResponse[];
}