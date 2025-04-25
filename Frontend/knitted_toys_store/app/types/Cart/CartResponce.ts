import { CartItemsResponce } from "../CartItems/CartItemsResponce";

export interface CartResponce {
  id: string;
  createAt: Date;
  lastUpdate: Date;
  totalAmount: number;
  cartItemsResponces: CartItemsResponce[];
  rowVersion: string;

  cartItems: CartItemsResponce[];
}
