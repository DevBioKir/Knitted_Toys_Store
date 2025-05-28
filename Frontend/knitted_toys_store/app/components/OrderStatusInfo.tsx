import { OrderStatus } from "../Models/Order";

export const statusInfo: Record<OrderStatus, {label: string, color: string}> = {
    [OrderStatus.Pending]: {label: "Ожидает оплаты", color: "orange"},
    [OrderStatus.Paid]: {label: "Оплачен", color: "green"},
    [OrderStatus.Shipped]: {label: "Отправлен", color: "blue"},
    [OrderStatus.Delivered]: {label: "Доставлен", color: "cyan"},
    [OrderStatus.Cancelled]: {label: "Отменен", color: "red"},
};