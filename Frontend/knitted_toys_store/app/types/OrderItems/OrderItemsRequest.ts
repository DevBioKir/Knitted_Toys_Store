export interface OrderItemsRequest {
    id?: string;
    orderId: string;
    toyId: string;
    quantity: number;
    priceAtTime: number;
}