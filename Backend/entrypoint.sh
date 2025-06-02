#!/bin/bash
set -e

echo "⏳ Ожидание готовности PostgreSQL..."

# Ждём, пока БД будет доступна
until dotnet ef database update --project Knitted_Toys_Store.API --startup-project Knitted_Toys_Store.API; do
  >&2 echo "❌ База данных недоступна - повтор через 3 секунды..."
  sleep 3
done

echo "✅ Миграции успешно применены!"

# Запуск приложения
exec dotnet Knitted_Toys_Store.API.dll