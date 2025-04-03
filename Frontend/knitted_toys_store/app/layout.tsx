"use client"
import { Layout, Menu } from "antd";
import Link from "next/link";
import "./globals.css";
import MenuItem from "antd/es/menu/MenuItem";

const {Header, Content, Footer} = Layout;
const menuItems = [

  { key: "1", label: <Link href="/">Главная</Link> },
  { key: "2", label: <Link href="/GetToys">Каталог игрушек</Link> },
  { key: "3", label: <Link href="/Carts">Корзина</Link> },
];

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ru">
      <body>
    <Layout className="min-h-screen">
      <Header className="bg-white shadow-md">
        <Menu mode="horizontal" items={menuItems} />
      </Header>
      <Content className="р-б">{children}</Content>
      <Footer className="text-center bg-gray-100 p-4">@ 2025 Магазин мягких игрушек Космический мишка</Footer>
    </Layout>
    </body>
    </html>
  );
}
