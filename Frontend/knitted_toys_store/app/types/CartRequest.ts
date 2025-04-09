import { CartItemsRequest } from "./CartItemsRequest"

export interface CartRequest {
    id?: string;
    createAt: string;
    lastUpdate: string;
    totalAmount: number;
    cartItems: CartItemsRequest[];
    rowVersion: string;
}