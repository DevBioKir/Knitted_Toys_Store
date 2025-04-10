import { CartRequest } from "../types/CartRequest";
import { CartResponce } from "../types/CartResponce";


export const getAllCarts = async (): Promise<CartResponce[]> => {
    const response = await fetch(`http://localhost:5237/Carts`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error("Failed to fetch carts");
    }

    const data: CartResponce[] = await response.json(); // Преобразуем ответ в массив объектов CartResponce
    return data; // Возвращаем данные
};