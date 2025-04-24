"use client";

import { message, Tabs } from "antd";
import AddToyPage from "../components/Admin/AddToys";
import { useState } from "react";
import ToysPage from "../toys/page";

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
                <ToysPage/>
            )
        }
    ]}
    />
    </div>
  );
}
