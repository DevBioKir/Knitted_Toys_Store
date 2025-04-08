import { CartItems } from "./CartItems";

interface Cart {
    id: string;
    createAt: Date;
    lastUpdate: Date;
    totalAmount: number;
    cartItems: CartItems[];
    rowVersion: string; // base64 строка
}