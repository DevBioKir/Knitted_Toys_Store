import { ToyRequest } from "../types/Toy/ToyRequest";
import { ToyResponse } from "../types/Toy/ToyResponse";


export const getAllToys = async (): Promise<ToyResponse[]> => {
    const url = `${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy`;
    console.log('🔍 Запрос к URL:', url);
    
    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: 'include',
    });
    
    console.log('📡 Статус ответа:', response.status);
    console.log('📡 Headers:', response.headers);
    
    if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Ошибка ответа:', errorText);
        throw new Error(`Failed to fetch toys: ${response.status} - ${errorText}`);
    }
    
    const data: ToyResponse[] = await response.json();
    console.log('✅ Полученные данные:', data);
    console.log('📊 Количество игрушек:', data.length);
    
    return data;
};
// export const getAllToys = async (): Promise<ToyResponse[]> => {
//     const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy`, {
//         method: "GET",
//         headers: {
//             "Content-Type": "application/json",
//         },
//         credentials: 'include',
//     });

//     if (!response.ok) {
//         throw new Error("Failed to fetch toys");
//     }

//     const data: ToyResponse[] = await response.json(); // Преобразуем ответ в массив объектов ToyResponse
//     return data; // Возвращаем данные
// };

export const getToyById = async (id: string): Promise<ToyResponse> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy/${id}`, {
        method: "GET",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }

    return response.json();
};

export const createToy = async(toyrequest: ToyRequest) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(toyrequest), 
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
}; 

export const updateToy = async(id: string, toyrequest: ToyRequest) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy/${id}`, {
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
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy/${id}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
};

// export async function uploadImage(file: File): Promise<string> {
//     if (!file) throw new Error("Файл не выбран");
    
//     const formData = new FormData();
//     formData.append("image", file); // Важно: ключ "file" должен совпадать с параметром в контроллере
  
//     const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/ImageUpload/upload`, {
//       method: "POST",
//       body: formData,
//       credentials: "include",
//       // Не указываем Content-Type — browser сам установит multipart/form-data с boundary
//     });
  
//     if (!response.ok) {
//       throw new Error(`Ошибка при загрузке изображения: ${response.statusText}`);
//     }
  
//     const data = await response.json();
//     return data.filePath; // вернёт, например: /Images/abc.jpg
//   }

