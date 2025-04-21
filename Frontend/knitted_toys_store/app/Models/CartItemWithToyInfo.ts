import { CartItemsResponce } from "../types/CartItems/CartItemsResponce";
import { ToyResponce } from "../types/Toy/ToyResponce";

export interface CartItemWithToyInfo {
    cartItem: CartItemsResponce;
    toy: ToyResponce | null;
}