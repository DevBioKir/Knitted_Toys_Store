"use client"
import { Layout, Menu } from "antd";
import Link from "next/link";
import '@ant-design/v5-patch-for-react-19'; // Импорт пакета для совместимости с React 19
import "./globals.css";
import MenuItem from "antd/es/menu/MenuItem";

const {Header, Content, Footer} = Layout;
const menuItems = [

  { key: "home", label: <Link href="/">Главная</Link> },
  { key: "Toys", label: <Link href="/toys">Каталог игрушек</Link> },
  { key: "cart", label: <Link href="/carts">Корзина</Link> },
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
          <div className="bg-gradient-to-r from-black via-indigo-900 to-black shadow-lg">
            <Header className="bg-transparent">
              <Menu
                theme="dark"
                mode="horizontal"
                items={menuItems}
                className="bg-transparent text-white font-semibold"
              />
            </Header>
          </div>
  
          <Content className="p-4">{children}</Content>

          <Footer className="text-center bg-gray-100 p-4">
            © 2025 Магазин мягких игрушек «Космический мишка»
          </Footer>
          {/* {children}*/}</Layout>
    </body>
    </html>
  );
}
