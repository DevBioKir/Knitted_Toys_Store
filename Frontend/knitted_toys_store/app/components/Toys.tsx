import { Button, Card } from "antd"
import { CardTitle } from "./CardTitle"

interface Props {
    toys: Toy[]
}

export const Toys = ({toys}: Props) => {
    return(
        <div className="cards">
            {toys.map((toy : Toy) => (
                <Card 
                    key={toy.id} 
                    title={<CardTitle name={toy.name} price={toy.price} />}
                    bordered ={false}
                    >
                        <p>{toy.description}</p>
                        <div className="card_buttons">
                            <Button>Изменить</Button>
                            <Button>Удалить</Button>
                        </div>;
                </Card>
            ))}
        </div>
    )
}