import { ToyRequest } from "../types/Toy/ToyRequest";
import { ToyResponce } from "../types/Toy/ToyResponce";


export const getAllToys = async (): Promise<ToyResponce[]> => {
    const response = await fetch(`http://localhost:5237/Toy`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error("Failed to fetch toys");
    }

    const data: ToyResponce[] = await response.json(); // Преобразуем ответ в массив объектов ToyResponce
    return data; // Возвращаем данные
};

export const getToyById = async (id: string): Promise<ToyResponce> => {
    const response = await fetch(`http://localhost:5237/Toy/${id}`, {
        method: "GET",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }

    return response.json();
};

export const createToy = async(toyrequest: ToyRequest) => {
    const response = await fetch("http://localhost:5237/Toy", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(toyrequest), 
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
}; 

export const updateToy = async(id: string, toyrequest: ToyRequest) => {
    const response = await fetch(`http://localhost:5237/Toy/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(toyrequest),
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
};

export const deleteToy = async(id: string) => {
    const response = await fetch(`http://localhost:5237/Toy/${id}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
};

