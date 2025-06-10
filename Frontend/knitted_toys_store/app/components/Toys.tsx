import { Card, Button, Space } from "antd";
import { Toy } from "../Models/Toy";
import styles from "./Toys.module.css";

interface Props {
  toys: Toy[];
  // carts: Cart[];
  //onEdit: (toy: Toy) => void;
  // onDelete: (id: string) => void;
  onAddToCart: (idToy: string) => void;
}

export const Toys = ({ toys, onAddToCart }: Props) => {
  //{/*onDelete*/}
  const baseUrl = process.env.NEXT_PUBLIC_DEV_API_BASE_URL;

  return (
    <div className={styles.container}>
      <div className={styles.grid}>
        {toys.map((toy) => (
          <Card
            key={toy.id}
            title={<span className={styles.toyName}>{toy.name}</span>}
            cover={
              toy.imageUrl ? (
                <img
                  alt={toy.name}
                  src={`${baseUrl}${toy.imageUrl}`}
                  className={styles.toyImage}
                />
              ) : null
            }
            className={styles.toyCard}
          >
            <p className={styles.toyDescription}>{toy.description}</p>
            <p>Размер: {toy.size}мм</p>
            <p>Цена: {toy.price} ₽</p>
            <Space>
              {/* <Button onClick={() => onEdit(toy)}>Редактировать</Button> */}
              <Button 
              className={styles.addToCartButton}
              onClick={() => onAddToCart(toy.id!)}>
                Добавить в корзину
              </Button>
            </Space>
          </Card>
        ))}
      </div>
    </div>
  );
};
