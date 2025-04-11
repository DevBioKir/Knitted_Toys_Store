import { CartRequest } from "../types/CartRequest";
import { CartResponce } from "../types/CartResponce";


export const getAllCarts = async (): Promise<CartResponce[]> => {
    const response = await fetch(`http://localhost:5237/Cart`, {
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

export const getCartById = async (id: string): Promise<CartResponce> => {
    const response = await fetch(`http://localhost:5237/Cart/${id}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error(`Ошибка при получении корзины с ID ${id}`);
    }

    return response.json();
};

export const createCart = async (cartRequest: CartRequest) => {
    const response = await fetch ("http://localhost:5237/Cart/CreateCart", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(cartRequest),
    });

    if (!response.ok) {
        throw new Error("Не удалось создать корзину");
    }
};

export const updateCart = async (cartId: string, cartRequest: CartRequest): Promise<CartResponce> => {
    const response = await fetch(`http://localhost:5237/Cart?cartId=${cartId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(cartRequest),
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Ошибка при обновлении корзины: ${errorText}`);
    }

    return await response.json();
};