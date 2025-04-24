"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

const AdminEasterEgg = () => {
    const [input, setInput] = useState("");
    const router = useRouter();

    useEffect(() => {
        const handleKeyPress = (event: KeyboardEvent) => {
            const char = event.key.toLowerCase();
            if (/^[a-z0-9]$/.test(char)) {
                const next = (input + char).slice(-5); // храним последние 5 символов
                setInput(next);
                if (next === "admin") {
                    // делаем редирект с задержкой, чтобы избежать конфликта с setState
                    setTimeout(() => {
                        router.push("/AdminController"); //тут поставить страницу админ контроля
                    }, 0);
                }
            }
        };

        window.addEventListener("keydown", handleKeyPress);
        return () => window.removeEventListener("keydown", handleKeyPress);
    }, [input, router]);

    return null;
};

export default AdminEasterEgg;