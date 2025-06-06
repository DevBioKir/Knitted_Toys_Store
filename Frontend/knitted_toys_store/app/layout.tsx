"use client";

import { Layout, Menu, Drawer, Button } from "antd";
import Link from "next/link";
import { MenuOutlined } from "@ant-design/icons";
import { useState, useEffect } from "react";
import "@ant-design/v5-patch-for-react-19";
import "./globals.css";
import CookieConsent from "react-cookie-consent";
import { CartProvider } from "./context/CartProvider";
import { OrderProvider } from "./context/OrderProvider";

const { Header, Content, Footer } = Layout;

const menuItems = [
  { key: "home", label: <Link href="/">Главная</Link> },
  { key: "Toys", label: <Link href="/toysPage">Каталог игрушек</Link> },
  { key: "cart", label: <Link href="/cartsPage">Корзина</Link> },
  { key: "orders", label: <Link href="/ordersPage">Мой заказ</Link> },
];

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const [isMobile, setIsMobile] = useState(false);
  const [drawerVisible, setDrawerVisible] = useState(false);

  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth <= 768);
    handleResize(); // Первичная проверка
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return (
    <html lang="ru">
      <body>
        <CartProvider>
          <OrderProvider>
            <Layout className="min-h-screen">
              <Header className="bg-gradient-to-r from-black via-indigo-900 to-black shadow-lg text-white flex justify-between items-center px-4">
                {isMobile ? (
                  <>
                    <Button
                      type="text"
                      icon={
                        <MenuOutlined
                          style={{ fontSize: "24px", color: "green" }}
                        />
                      }
                      onClick={() => setDrawerVisible(true)}
                    />
                    <Drawer
                      title="Меню"
                      placement="left"
                      onClose={() => setDrawerVisible(false)}
                      open={drawerVisible}
                    >
                      <Menu
                        mode="vertical"
                        items={menuItems}
                        onClick={() => setDrawerVisible(false)}
                      />
                    </Drawer>
                  </>
                ) : (
                  <Menu
                    theme="dark"
                    mode="horizontal"
                    items={menuItems}
                    className="bg-transparent text-white font-semibold"
                  />
                )}
              </Header>

              <Content className="p-4">{children}</Content>

              <Footer className="text-center bg-gray-100 p-4">
                © 2025 Магазин мягких игрушек «Космический мишка»
              </Footer>
            </Layout>

            <CookieConsent
              location="bottom"
              buttonText="Принять"
              cookieName="cookie_cartId"
              style={{
                background: "#2B373B",
                color: "#fff",
                fontSize: "14px",
                textAlign: "center",
                padding: "10px",
              }}
              buttonStyle={{
                background: "#f1d600",
                color: "#000",
                borderRadius: "5px",
                fontSize: "13px",
                padding: "8px 16px",
              }}
            >
              Мы используем cookies для улучшения вашего опыта на сайте.
              Продолжая использовать сайт, вы соглашаетесь на использование
              cookies.
            </CookieConsent>
          </OrderProvider>
        </CartProvider>
      </body>
    </html>
  );
}
