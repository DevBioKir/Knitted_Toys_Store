export interface CartItemsResponse {
    id: string;
    cartId: string;
    toyId: string;
    quantity: number;
    addedAt: Date;
    toyName?: string;
    toyImageUrl?: string;
}