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
          <p>Размер: {toy.size}</p>
          <p>Цена: {toy.price} ₽</p>
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




// import { Button, Card } from "antd"
// import { CardTitle } from "./CardTitle"
// import { Toy } from "../Models/Toy";

// interface Props {
//     toys: Toy[]
// }

// export const Toys = ({ toys }: Props) => {
//     return (
//       <div className="cards">
//         {toys.length === 0 ? (
//           <p>Нет игрушек для отображения.</p>
//         ) : (
//           toys.map((toy: Toy) => (
//             <Card 
//             key={toy.id} 
//             title={
//               <CardTitle 
//                 name={toy.name} 
//                 price={toy.price} 
//                 size={toy.size}
//             />}>

//               <p>{toy.description}</p>
//               <div className="card_buttons">
//                 <Button>Изменить</Button>
//                 <Button>Удалить</Button>
//               </div>
//             </Card>
//           ))
//         )}
//       </div>
//     );
//   };