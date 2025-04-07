export interface ToyRequest{
    name: string;
    description: string;
    size: string;
    price: number;
    imageUrl: string;
}

export const getAllToys = async() => {
    const response = await fetch("http://localhost:5237/Toy");

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }

    const data = await response.json();
    console.log("Данные с сервера:", data);
    return data;
};

export const getToyById = async (id: string) => {
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
