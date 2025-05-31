import { useEffect, useState } from 'react';

const CookieNotice = () => {
  const [showNotice, setShowNotice] = useState(false);

  useEffect(() => {
    // Проверяем, есть ли cookie с cartId
    const cartId = document.cookie.split('; ').find(row => row.startsWith('cart_id='));

    if (!cartId) {
      // Если cookie нет, показываем уведомление
      setShowNotice(true);
    }
  }, []);

  const handleAccept = () => {
    setShowNotice(false);

    // Добавить код для установки cookie
  };

  return (
    showNotice && (
      <div
        style={{
          position: 'fixed',
          bottom: '10px',
          left: '140px',
          padding: '20px',
          backgroundColor: 'rgba(8, 141, 74, 0.8)',
          color: 'black',
          borderRadius: '20px',
          zIndex: 1000,
        }}
      >
        <p>
          We use cookies to improve your experience. By continuing to browse, you accept our use of cookies.
        </p>
        <button
          onClick={handleAccept}
          style={{
            backgroundColor: '#00FA9A',
            color: 'black',
            padding: '5px 10px',
            border: 'none',
            borderRadius: '5px',
            cursor: 'pointer',
          }}
        >
          Accept
        </button>
      </div>
    )
  );
};

export default CookieNotice;