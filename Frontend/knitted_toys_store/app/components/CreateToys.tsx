import { Input, Modal } from "antd";
import { ToyRequest } from "../services/toys";
import { title } from "process";
import { useState } from "react";

interface Prop {
  mode: Mode;
  values: Toy[];
  isModalOpen: boolean;
  handleCancel: () => void;
  handleCreate: (request: ToyRequest) => void;
}

export enum Mode {
  Create,
}
export const CreateToys = ({
  mode,
  values,
  isModalOpen,
  handleCancel,
  handleCreate,
}: Prop) => {
    const[name, setName] = useState<string>("");
    const[description, setDescription] = useState<string>("");
    const[size, setSize] = useState<string>("");
    const[price, setPrice] = useState<number>(1);
    const[imageUrl, setImageUrl] = useState<string>("");


  return (
    <Modal
      title={
        mode === Mode.Create ? "Добавить игрушку" : "Редактировать игрушку"
      }
      open={isModalOpen}
      cancelText={"Отмена"}
    >
        <div className="toy_model">
            <Input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder={"Имя игрушки"}
            />
            <Input
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder={"Описание"}
            />
            <Input
            value={size}
            onChange={(e) => setSize(e.target.value)}
            placeholder={"Размер"}
            />
            <Input
            value={price}
            onChange={(e) => setPrice(Number(e.target.value))}
            placeholder={"Цена"}
            />
            <Input
            value={imageUrl}
            onChange={(e) => setImageUrl(e.target.value)}
            placeholder={"Цена"}
            />
        </div>
    </Modal>
  );
};
