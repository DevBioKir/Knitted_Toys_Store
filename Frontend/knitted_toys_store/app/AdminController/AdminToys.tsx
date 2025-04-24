//"use client"

import { Toy } from "@/app/Models/Toy"
import { deleteToy, getAllToys } from "@/app/services/toys";
import { message } from "antd";
import { useEffect, useState } from "react"

export default function AdminToysPage() {
    const [toys, setToys] = useState<Toy[]>([]);
    const [loading, setLoading] = useState(true);
    const [editingToys, setEditingToys] = useState<Toy | null>(null);

    const fetchToys = async () => {
        try{
            const data = await getAllToys();
            setToys(data);
        }catch (err) {
            message.error("Ошибка при загрузке игрушек");
        }finally{
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchToys()
    }, []);

    const handleDelete = async (id: string) => {
        try{
            await deleteToy(id);
            message.success("Игрушка удалена");
            fetchToys();
        }catch (err) {
            message.error("Не удалось удалить игрушку")
        }
    };

    const handleUpdate = async (updateToy: Toy) => {
        try{

        }

    }
}