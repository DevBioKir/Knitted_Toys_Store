import { CartItemsResponse } from "../CartItems/CartItemsResponse";

export interface CartResponse {
  id: string;
  createAt: Date;
  lastUpdate: Date;
  totalAmount: number;
  cartItemsResponses: CartItemsResponse[];
  rowVersion: string;

  cartItems: CartItemsResponse[];
}
