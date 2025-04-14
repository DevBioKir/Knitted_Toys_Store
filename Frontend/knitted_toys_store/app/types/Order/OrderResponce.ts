import { OrderStatus } from "../../Models/Order";
import { OrderItemsResponce } from "../OrderItems/OrderItemsResponce";

export interface OrderResponce{
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
    orderItemsRequest: OrderItemsResponce;
}