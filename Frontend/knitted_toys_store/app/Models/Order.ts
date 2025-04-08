import {OrderItems} from "./OrderItems"

interface Order {
    id: string;
    createAt: Date;
    lastUpdate: Date;
    totalAmount: number;
}