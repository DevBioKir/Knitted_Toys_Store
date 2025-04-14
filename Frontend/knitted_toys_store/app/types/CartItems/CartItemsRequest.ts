export interface CartItemsRequest {
    id?: string;
    cartId: string;
    toyId: string;
    quantity: number;
    addedAt: Date;
}