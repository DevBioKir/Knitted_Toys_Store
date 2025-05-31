import { CartItemsResponse } from "../types/CartItems/CartItemsResponse";
import { ToyResponse } from "../types/Toy/ToyResponse";

export interface CartItemWithToyInfo {
    cartItem: CartItemsResponse;
    toy: ToyResponse | null;
}