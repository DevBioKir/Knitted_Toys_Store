export interface OrderItemsResponse {
    id: string;
    orderId: string;
    toyId: string;
    quantity: number;
    priceAtTime: number;
    toyName?: string;
    toyImageUrl?: string;
}