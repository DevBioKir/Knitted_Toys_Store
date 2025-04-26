"use client";

import { message, Tabs } from "antd";
import AddToyPage from "../components/Admin/Toys/AddToys";
import { useState } from "react";
import ToysPage from "../toys/page";
import AdminToysPage from "./Toys/AdminToys";
import CartPage from "../carts/page";
import AdminCartsPage from "./Carts/AdminCarts";

export default function AdminPage() {
  const [activeTable, setActiveTable] = useState("toys");

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
                <AddToyPage
                onToyCreated={ () => {
                    message.success("Игрушка успешно добавлена");
                    setActiveTable("toys");
                }}
                />
            ),
        },
        {
            key: "toys",
            label: "Каталог игрушек",
            children: (
                <AdminToysPage/>
            )
        },
        {
          key: "carts",
          label: "Корзина",
          children: (
            <AdminCartsPage />
          )
        },
        
    ]}
    />
    </div>
  );
}
