"use client";

import React, { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { getAllCarts, getCartById, updateCart } from "../services/carts";
import { CartResponce } from "../types/CartResponce";
import { CartRequest } from "../types/CartRequest";
import "../styles/CartPage.css";

const CartPage: React.FC = () => {
  const [cart, setCart] = useState<CartResponce | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  // Получаем ID корзины из cookie
  const getCartIdFromCookie = (): string | null => {
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
      const cookie = cookies[i].trim();
      if (cookie.startsWith('cart_id=')) {
        return cookie.substring('cart_id='.length, cookie.length);
      }
    }
    return null;
  };

  useEffect(() => {
    const fetchCart = async () => {
      try {
        setLoading(true);
        const cartId = getCartIdFromCookie();
        
        if (!cartId) {
          // Если ID корзины нет в cookie, получаем все корзины и берем первую
          // (это временное решение, в реальном приложении лучше создать новую корзину)
          const carts = await getAllCarts();
          if (carts.length > 0) {
            setCart(carts[0]);
          } else {
            setError('Корзина не найдена');
          }
        } else {
          // Если ID корзины есть в cookie, получаем корзину по ID
          const cartData = await getCartById(cartId);
          setCart(cartData);
        }
      } catch (err) {
        setError('Не удалось загрузить корзину. Пожалуйста, попробуйте позже.');
        console.error('Error fetching cart:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchCart();
  }, []);

  const handleQuantityChange = async (toyId: string, newQuantity: number) => {
    if (!cart) return;
    
    try {
      if (newQuantity < 1) return;
      
      // Создаем обновленную корзину
      const updatedCartItems = cart.cartItemsResponces.map(item => 
        item.toyId === toyId 
          ? { ...item, quantity: newQuantity } 
          : item
      );
      
      // Рассчитываем новую общую сумму (в реальном приложении это должно делаться на сервере)
      // Здесь предполагается, что у нас есть доступ к ценам товаров
      // В реальном приложении нужно получить цены с сервера или хранить их в состоянии
      
      // Создаем объект запроса
      const cartRequest: CartRequest = {
        id: cart.id,
        createAt: cart.createAt,
        lastUpdate: new Date().toISOString(),
        totalAmount: cart.totalAmount, // В реальном приложении пересчитывать сумму
        cartItems: updatedCartItems.map(item => ({
          id: item.id,
          cartId: item.cartId,
          toyId: item.toyId,
          quantity: item.quantity,
          addedAt: new Date(item.addedAt)
        })),
        rowVersion: cart.rowVersion
      };
      
      // Обновляем корзину на сервере
      const updatedCart = await updateCart(cart.id, cartRequest);
      setCart(updatedCart);
    } catch (err) {
      setError('Не удалось обновить количество товара. Пожалуйста, попробуйте позже.');
      console.error('Error updating quantity:', err);
    }
  };

  const handleRemoveItem = async (toyId: string) => {
    if (!cart) return;
    
    try {
      // Создаем обновленную корзину без удаляемого товара
      const updatedCartItems = cart.cartItemsResponces.filter(item => item.toyId !== toyId);
      
      // Создаем объект запроса
      const cartRequest: CartRequest = {
        id: cart.id,
        createAt: cart.createAt,
        lastUpdate: new Date().toISOString(),
        totalAmount: cart.totalAmount, // В реальном приложении пересчитывать сумму
        cartItems: updatedCartItems.map(item => ({
          id: item.id,
          cartId: item.cartId,
          toyId: item.toyId,
          quantity: item.quantity,
          addedAt: new Date(item.addedAt)
        })),
        rowVersion: cart.rowVersion
      };
      
      // Обновляем корзину на сервере
      const updatedCart = await updateCart(cart.id, cartRequest);
      setCart(updatedCart);
    } catch (err) {
      setError('Не удалось удалить товар. Пожалуйста, попробуйте позже.');
      console.error('Error removing item:', err);
    }
  };

  const handleCheckout = () => {
    // Переход на страницу оформления заказа
    router.push('/checkout');
  };

  if (loading) {
    return (
      <div className="cart-page">
        <div className="cart-loading">
          <div className="spinner"></div>
          <p>Загрузка корзины...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="cart-page">
        <div className="cart-error">
          <h2>Произошла ошибка</h2>
          <p>{error}</p>
          <button onClick={() => window.location.reload()} className="retry-button">
            Попробовать снова
          </button>
        </div>
      </div>
    );
  }

  if (!cart || cart.cartItemsResponces.length === 0) {
    return (
      <div className="cart-page">
        <div className="empty-cart">
          <h2>Ваша корзина пуста</h2>
          <p>Добавьте товары в корзину, чтобы продолжить покупки</p>
          <Link href="/catalog" className="continue-shopping-btn">
            Перейти в каталог
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <h1>Корзина</h1>
      
      <div className="cart-content">
        <div className="cart-items">
          <div className="cart-header">
            <div className="cart-header-product">Товар</div>
            <div className="cart-header-price">Цена</div>
            <div className="cart-header-quantity">Количество</div>
            <div className="cart-header-total">Сумма</div>
            <div className="cart-header-actions"></div>
          </div>
          
          {cart.cartItemsResponces.map((item) => (
            <CartItem 
              key={item.id}
              item={item}
              onQuantityChange={handleQuantityChange}
              onRemove={handleRemoveItem}
            />
          ))}
        </div>
        
        <div className="cart-summary">
          <h2>Ваш заказ</h2>
          
          <div className="summary-details">
            <div className="summary-row">
              <span>Товары ({cart.cartItemsResponces.reduce((sum, item) => sum + item.quantity, 0)})</span>
              <span>{cart.totalAmount.toLocaleString()} ₽</span>
            </div>
            
            <div className="summary-row">
              <span>Доставка</span>
              <span>
                {cart.totalAmount >= 3000 ? 'Бесплатно' : '300 ₽'}
              </span>
            </div>
            
            {cart.totalAmount < 3000 && (
              <div className="free-delivery-note">
                Добавьте товаров еще на {(3000 - cart.totalAmount).toLocaleString()} ₽ для бесплатной доставки
              </div>
            )}
            
            <div className="summary-total">
              <span>Итого</span>
              <span>{(cart.totalAmount + (cart.totalAmount >= 3000 ? 0 : 300)).toLocaleString()} ₽</span>
            </div>
          </div>
          
          <button className="checkout-btn" onClick={handleCheckout}>
            Оформить заказ
          </button>
          
          <div className="promo-code">
            <input 
              type="text" 
              placeholder="Введите промокод" 
              className="promo-input"
            />
            <button className="apply-promo-btn">Применить</button>
          </div>
        </div>
      </div>
      
      <div className="cart-actions">
        <Link href="/catalog" className="continue-shopping-btn">
          Продолжить покупки
        </Link>
      </div>
    </div>
  );
};

// Компонент элемента корзины
interface CartItemProps {
  item: CartResponce['cartItemsResponces'][0];
  onQuantityChange: (toyId: string, quantity: number) => void;
  onRemove: (toyId: string) => void;
}

const CartItem: React.FC<CartItemProps> = ({ item, onQuantityChange, onRemove }) => {
  // В реальном приложении здесь должна быть информация о товаре (название, цена, изображение)
  // Для примера используем заглушки
  const toy = {
    name: `Игрушка ${item.toyId.substring(0, 8)}`,
    price: 1000, // Заглушка для цены
    imageUrl: 'https://via.placeholder.com/100x100'
  };
  
  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newQuantity = parseInt(e.target.value, 10);
    if (!isNaN(newQuantity) && newQuantity > 0) {
      onQuantityChange(item.toyId, newQuantity);
    }
  };
  
  const handleIncrement = () => {
    onQuantityChange(item.toyId, item.quantity + 1);
  };
  
  const handleDecrement = () => {
    if (item.quantity > 1) {
      onQuantityChange(item.toyId, item.quantity - 1);
    }
  };
  
  return (
    <div className="cart-item">
      <div className="cart-item-product">
        <img src={toy.imageUrl} alt={toy.name} className="cart-item-image" />
        <div className="cart-item-details">
          <h3 className="cart-item-name">{toy.name}</h3>
          <p className="cart-item-id">Артикул: {item.toyId.substring(0, 8)}</p>
        </div>
      </div>
      
      <div className="cart-item-price">
        {toy.price.toLocaleString()} ₽
      </div>
      
      <div className="cart-item-quantity">
        <div className="quantity-control">
          <button 
            className="quantity-btn" 
            onClick={handleDecrement}
            disabled={item.quantity <= 1}
          >
            -
          </button>
          <input 
            type="number" 
            min="1" 
            value={item.quantity}
            onChange={handleQuantityChange}
            className="quantity-input"
          />
          <button 
            className="quantity-btn" 
            onClick={handleIncrement}
          >
            +
          </button>
        </div>
      </div>
      
      <div className="cart-item-total">
        {(toy.price * item.quantity).toLocaleString()} ₽
      </div>
      
      <div className="cart-item-actions">
        <button className="remove-item-btn" onClick={() => onRemove(item.toyId)}>
          <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M2 4H3.33333H14" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
            <path d="M5.33334 4V2.66667C5.33334 2.31305 5.47381 1.97391 5.72386 1.72386C5.97391 1.47381 6.31305 1.33334 6.66667 1.33334H9.33334C9.68696 1.33334 10.0261 1.47381 10.2761 1.72386C10.5262 1.97391 10.6667 2.31305 10.6667 2.66667V4M12.6667 4V13.3333C12.6667 13.687 12.5262 14.0261 12.2761 14.2761C12.0261 14.5262 11.687 14.6667 11.3333 14.6667H4.66667C4.31305 14.6667 3.97391 14.5262 3.72386 14.2761C3.47381 14.0261 3.33334 13.687 3.33334 13.3333V4H12.6667Z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
          </svg>
        </button>
      </div>
    </div>
  );
};

export default CartPage;