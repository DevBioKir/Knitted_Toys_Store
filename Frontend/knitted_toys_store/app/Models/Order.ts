import {OrderItems} from "./OrderItems"

export enum OrderStatus {
    Pending = "Pending",
    Paid = "Paid",
    Shipped = "Shipped",
    Delivered = "Delivered",
    Cancelled = "Cancelled",
}

export interface Order {
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
    orderItems: OrderItems;
}