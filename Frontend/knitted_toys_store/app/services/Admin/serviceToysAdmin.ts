
import { ToyRequest } from "@/app/types/Toy/ToyRequest";
import { ToyResponce } from "@/app/types/Toy/ToyResponce";
import adminAPI from "./adminAPI";


export const getAllToysAdmin = async (): Promise<ToyResponce[]> => {
    const response = await adminAPI("/AdminToy")
    return response.data; // Возвращаем данные
};


// export const getAllToysAdmin = async (): Promise<ToyResponce[]> => {
//     const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToy`, {
//         method: "GET",
//         headers: {
//             "Content-Type": "application/json",
//         },
//         credentials: 'include',
//     });

//     if (!response.ok) {
//         throw new Error("Failed to fetch toys");
//     }

//     const data: ToyResponce[] = await response.json(); // Преобразуем ответ в массив объектов ToyResponce
//     return data; // Возвращаем данные
// };

export const getToyByIdAdmin = async (id: string): Promise<ToyResponce> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToy/${id}`, {
        method: "GET",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }

    return response.json();
};

export const createToyAdmin = async(toyrequest: ToyRequest) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToy`, {
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

export const updateToyAdmin = async(id: string, toyrequest: ToyRequest) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToy/${id}`, {
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

export const deleteToyAdmin = async(id: string) => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToyks/${id}`, {
        method: "DELETE",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }
};

export async function uploadImage(file: File): Promise<string> {
    if (!file) throw new Error("Файл не выбран");
    
    const formData = new FormData();
    formData.append("image", file); // Важно: ключ "file" должен совпадать с параметром в контроллере
  
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/ImageUpload/upload`, {
      method: "POST",
      body: formData,
      credentials: "include",
      // Не указываем Content-Type — browser сам установит multipart/form-data с boundary
    });
  
    if (!response.ok) {
      throw new Error(`Ошибка при загрузке изображения: ${response.statusText}`);
    }
  
    const data = await response.json();
    return data.filePath; // вернёт, например: /Images/abc.jpg
  }

