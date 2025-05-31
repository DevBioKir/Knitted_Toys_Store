import { CartItemsResponse } from "../types/CartItems/CartItemsResponse";
import { CartItems } from "./CartItems";


export interface Cart {
    id?: string;
    createAt: Date;
    lastUpdate: Date;
    totalAmount: number;
    cartItems?: CartItems[];
    rowVersion: string; // base64 строка

    cartItemsResponses: CartItemsResponse[];
}