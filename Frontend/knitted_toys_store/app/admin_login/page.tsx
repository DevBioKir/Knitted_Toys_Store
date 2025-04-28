"use client"

import { adminAPI } from "@/app/services/Admin/adminAPI";
import { Button, Input, message } from "antd";
import { useRouter } from "next/navigation";
import { useState } from "react";

export default function AdminLoginForm() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const router = useRouter();

    const handleLogin = async() => {
        setLoading(true);
        try{
            const token = Buffer.from(`${username}:${password}`).toString("base64");

            await adminAPI.get("/AdminToy", {
                headers: { Authorization: `Basic ${token}` }
            });

            //Если запрос успешный, сохраняем token в localStorage
            localStorage.setItem("admin_token", token);

            message.success("Успешный вход в админку")
            router.push("/AdminController");
        } catch (error) {
            message.error("Ошибка авторизации");
          } finally {
            setLoading(false);
          }
    };

    return(
        <div style={{ maxWidth: 400, margin: "50px auto" }}>
      <h2>Вход в админку</h2>
      <Input
        placeholder="Логин"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        style={{ marginBottom: 10 }}
      />
      <Input.Password
        placeholder="Пароль"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        style={{ marginBottom: 20 }}
      />
      <Button type="primary" onClick={handleLogin} loading={loading} block>
        Войти
      </Button>
    </div>
  );
}