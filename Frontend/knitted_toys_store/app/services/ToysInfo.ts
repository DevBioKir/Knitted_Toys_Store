import { ToyResponse } from "../types/Toy/ToyResponse";

// Кэш для хранения данных об игрушках
const toyCache: Record<string, ToyResponse> = {};

export const getToyById = async (id: string): Promise<ToyResponse | null> => {
  // Проверяем кэш
  if (toyCache[id]) {
    return toyCache[id];
  }

  try {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/Toy/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });

    if (!response.ok) {
      console.error(`Ошибка при получении игрушки с ID ${id}: ${response.status}`);
      return null;
    }

    const data: ToyResponse = await response.json();
    
    // Сохраняем в кэш
    toyCache[id] = data;
    
    return data;
  } catch (error) {
    console.error(`Ошибка при получении игрушки с ID ${id}:`, error);
    return null;
  }
};

export const getToysByIds = async (ids: string[]): Promise<Record<string, ToyResponse>> => {
  const result: Record<string, ToyResponse> = {};
  
  // Получаем уникальные ID
  const uniqueIds = [...new Set(ids)];
  
  // Создаем массив промисов для параллельного выполнения запросов
  const promises = uniqueIds.map(async (id) => {
    const toy = await getToyById(id);
    if (toy) {
      result[id] = toy;
    }
  });
  
  // Ждем выполнения всех запросов
  await Promise.all(promises);
  
  return result;
};