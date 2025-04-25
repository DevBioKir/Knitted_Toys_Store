"use client"

import { Cart } from "@/app/Models/Cart";
import { getAllCarts } from "@/app/services/carts";
import { message } from "antd";
import { useEffect, useState } from "react"

export default function AdminCartsPage() {
    const [carts, setCarts] = useState<Cart[]>([]);
    const [loading, setLoading] = useState(true);
    const [editingCart, setEditingCart] = useState<Cart | null>(null);

    const fetchCart = async () => {
        try{
            const data = await getAllCarts();
            setCarts(data);
        } catch(err) {
            console.error(err);
            message.success("Произошла ошибка при загрузке корзин");
        }
    };

    useEffect(() => {
        fetchCart();
    }, [])

    const handleDelete = async (id: string) => {
        try{
            await delete
        }
    }
}