import { useState } from "react";
import { Order } from "../Models/Order";

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);


  
}