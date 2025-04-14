import { CartItemsResponce } from "../CartItems/CartItemsResponce";

export interface CartResponce {
  id: string;
  createAt: string;
  lastUpdate: string;
  totalAmount: number;
  cartItemsResponces: CartItemsResponce[];
  rowVersion: string;
}