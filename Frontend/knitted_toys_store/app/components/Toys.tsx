import { Card, Button, Space } from "antd";
import { Toy } from "../Models/Toy";

interface Props {
  toys: Toy[];
  onEdit: (toy: Toy) => void;
  onDelete: (id: string) => void;
}

export const Toys = ({ toys, onEdit, onDelete }: Props) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {toys.map((toy) => (
        <Card
          key={toy.id}
          title={toy.name}
          cover={<img alt={toy.name} src={toy.imageUrl} />}
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