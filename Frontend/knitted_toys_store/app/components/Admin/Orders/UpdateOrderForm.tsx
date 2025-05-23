"use client";

import { getAllOrdersAdmin } from "@/app/services/Admin/serviceOrdersAdmin";
import { OrderRequest } from "@/app/types/Order/OrderRequest";
import { OrderResponse } from "@/app/types/Order/OrderResponce";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";
import { Form } from "antd";
import { b } from "framer-motion/client";
import { useEffect, useState } from "react";

interface Props {
    order: OrderResponse;
    onSuccess: () => void;
}

export const UpdateOrderForm = ({ order, onSuccess }: Props) => {
    const [ form ] = Form.useForm<OrderRequest>();
    const [orders, setOrders] = useState<ToyResponse[]>();
    const [selectedToy, setSelectedToy] = useState<string | null>(null);
    const [quantity, setQuantity] = useState<number>(1);

    useEffect(() => {
        getAllOrdersAdmin()
        .then((orders) => setOrders([...orders].sort(a,b) => a.name.localeCompare(b.name))))
        .catch(() => message.error("Не удалось загрузить игрушки"));
    })
}