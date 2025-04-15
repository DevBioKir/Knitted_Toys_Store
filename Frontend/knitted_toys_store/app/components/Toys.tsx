import { Card, Button, Space } from "antd";
import { Toy } from "../Models/Toy";

interface Props {
  toys: Toy[];
  onEdit: (toy: Toy) => void;
  onDelete: (id: string) => void;
}

export const Toys = ({ toys, onEdit, onDelete }: Props) => {
  return (
    <div className="h-full flex flex-col">
      {toys.map((toy) => (
        <Card
        key={toy.id}
        title={toy.name}
        cover={
          <img
            alt={toy.name}
            src={toy.imageUrl}
            style={{ height: 200, objectFit: "cover" }}
          />
        }
        style={{ width: 300 }}
      >
          <p>{toy.description}</p>
          <p>Размер: {toy.size}мм</p>
          <p>Цена: {toy.price} ₽</p>
          {/*<p>Ссылка на изображение: {toy.imageUrl}</p>*/}
          <Space>
            <Button onClick={() => onEdit(toy)}>Редактировать</Button>
            <Button danger onClick={() => onDelete(toy.id!)}>
              Удалить
            </Button>
          </Space>
        </Card>
      ))}
    </div>
  );
};