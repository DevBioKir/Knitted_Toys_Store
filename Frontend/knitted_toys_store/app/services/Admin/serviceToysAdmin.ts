
import { ToyRequest } from "@/app/types/Toy/ToyRequest";
import adminAPI from "./adminAPI";
import { ToyResponse } from "@/app/types/Toy/ToyResponse";

export const getAllToysAdmin = async (): Promise<ToyResponse[]> => {
    try{
        const response = await adminAPI.get("/AdminToy")
        return response.data;
    } catch(err) {
        console.error("Ошибка при поиске всех игрушек", err);
        throw err;
    }
};

export const getToyByIdAdmin = async (id: string): Promise<ToyResponse> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/AdminToy/${id}`, {
        method: "GET",
    });

    if (!response.ok) {
        throw new Error(`Ошибка: ${response.status}`);
    }

    return response.json();
};

export const createToyAdmin = async(toyRequest: ToyRequest) => {
    try{
        const response = await adminAPI.post("/AdminToy", toyRequest);
    } catch(err) {
        console.error("Ошибка при создании игрушки", err);
        throw err;
    }
}; 

export const updateToyAdmin = async(id: string, toyRequest: ToyRequest) => {
    try{
        const response = await adminAPI.put(`/AdminToy/${id}`, toyRequest);
    } catch(err) {
        console.error("Ошибка при измении игрушки", err);
        throw err;
    }
};

export const deleteToyAdmin = async(id: string) => {
    try{
        const response = await adminAPI.delete(`/AdminToy/${id}`);
    } catch(err) {
        console.error("Ошибка при удалении игрушки", err);
        throw err;
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
    });
  
    if (!response.ok) {
      throw new Error(`Ошибка при загрузке изображения: ${response.statusText}`);
    }
  
    const data = await response.json();
    return data.filePath;
  }

