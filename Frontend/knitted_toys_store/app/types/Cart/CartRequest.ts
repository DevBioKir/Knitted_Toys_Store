import { CartItemsRequest } from "../CartItems/CartItemsRequest";

export interface CartRequest {
  id?: string;
  createAt: Date;
  lastUpdate: Date;
  totalAmount: number;
  cartItemsRequest: CartItemsRequest[];
  rowVersion: string;
}
