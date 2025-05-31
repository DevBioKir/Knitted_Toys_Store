import { Card, Button, Space } from "antd";
import { Toy } from "../Models/Toy";

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
    <div style={{ padding: "20px" }}>
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
          gap: "16px",
          maxWidth: "1200px",
          margin: "0 auto",
        }}
      >
        {toys.map((toy) => (
          <Card
            key={toy.id}
            title={toy.name}
            cover={
              toy.imageUrl ? (
                <img
                  alt={toy.name}
                  src={`${baseUrl}${toy.imageUrl}`}
                  style={{ height: 200, objectFit: "cover" }}
                />
              ) : null
            }
            style={{ width: 300 }}
          >
            <p>{toy.description}</p>
            <p>Размер: {toy.size}мм</p>
            <p>Цена: {toy.price} ₽</p>
            <Space>
              {/* <Button onClick={() => onEdit(toy)}>Редактировать</Button> */}
              <Button onClick={() => onAddToCart(toy.id!)}>
                Добавить в корзину
              </Button>
            </Space>
          </Card>
        ))}
      </div>
    </div>
  );
};
