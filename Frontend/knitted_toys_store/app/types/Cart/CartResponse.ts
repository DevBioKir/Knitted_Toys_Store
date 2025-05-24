import { CartItemsResponse } from "../CartItems/CartItemsResponse";

export interface CartResponse {
  id?: string;
  createAt: Date;
  lastUpdate: Date;
  totalAmount: number;
  cartItems?: CartItemsResponse[];
  rowVersion: string;
  
  cartItemsResponses: CartItemsResponse[];
}
