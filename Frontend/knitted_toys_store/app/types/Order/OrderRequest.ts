import { OrderStatus } from "../../Models/Order";
import { OrderItemsRequest } from "../OrderItems/OrderItemsRequest";

export interface OrderRequest{
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
    orderItemsRequest: OrderItemsRequest;
}