"use client";

import { message, Tabs } from "antd";
import AddToyPage from "../components/Admin/Toys/AddToys";
import { useEffect, useState } from "react";
import AdminToysPage from "./Toys/AdminToys";
import AdminCartsPage from "./Carts/AdminCarts";
import ToyUploadPage from "../components/Admin/Toys/UploadToy";
import { useRouter } from "next/navigation";
import "./../components/Admin/Toys/AdminAddToyPage.css";

export default function AdminPage() {
  const [activeTable, setActiveTable] = useState("toys");
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("admin_token");

    if (!token) {
      router.replace("/admin_login");
    } else {
      setLoading(false);
    }
  }, [router]);

  if (loading) {
    return <div>Проверка доступа...</div>;
  }

  return (
    <div style={{ padding: 24, maxWidth: 1200, margin: "0 auto" }}>
      <h1>Панель администратора</h1>
      <Tabs
        activeKey={activeTable}
        onChange={setActiveTable}
        items={[
          {
            key: "add",
            label: "Добавить игрушку",
            children: (
              <div className="admin-add-container">
                <div className="admin-add-left">
                  <h2>Добавить вручную</h2>
                  <AddToyPage
                    onToyCreated={() => {
                      message.success("Игрушка успешно добавлена");
                      setActiveTable("toys");
                    }}
                  />
                </div>
                <div className="admin-add-right">
                  <h2>Загрузить с помощью Excel</h2>
                  <ToyUploadPage />
                </div>
              </div>
            ),
          },
          {
            key: "toys",
            label: "Каталог игрушек",
            children: <AdminToysPage />,
          },
          {
            key: "carts",
            label: "Корзина",
            children: <AdminCartsPage />,
          },
        ]}
      />
    </div>
  );
}
