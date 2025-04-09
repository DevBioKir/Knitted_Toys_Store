import { OrderStatus } from "../Models/Order";
import { OrderItemsRequest } from "./OrderItemsRequest";

export interface OrderRequest{
    id?: string;
    odredDate: string;
    totalAmount: string;
    status: OrderStatus;
    surnameCustomer: string;
    nameCustomer: string;
    phoneNumber: string;
    email: string;
    deliveryAddress: string;
    deliveryNotes: string;
    orderItemsRequest: OrderItemsRequest;
}