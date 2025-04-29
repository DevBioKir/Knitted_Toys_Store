import { OrderStatus } from "../../Models/Order";
import { OrderItemsResponse } from "../OrderItems/OrderItemsResponce";


export interface OrderResponse{
    id: string;
    odredDate: string;
    totalAmount: string;
    status: OrderStatus;
    surnameCustomer: string;
    nameCustomer: string;
    phoneNumber: string;
    email: string;
    deliveryAddress: string;
    deliveryNotes: string;
    orderItemsRequest: OrderItemsResponse;
}