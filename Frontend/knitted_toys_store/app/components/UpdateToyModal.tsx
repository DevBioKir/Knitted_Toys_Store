import { Modal } from "antd";
import { Toy } from "../Models/Toy";
import { ToyRequest } from "../types/Toy/ToyRequest";
import { ToyForm } from "./ToyForm";

interface Prop {
  toy: Toy;
  isOpen: boolean;
  onCancel: () => void;
  onUpdate: (id: string, toy: ToyRequest) => void;
}

export const UpdateToyModal = ({ toy, isOpen, onCancel, onUpdate }: Prop) => {
  return (
    <Modal
      title="Редактирование игрушки"
      open={isOpen}
      onCancel={onCancel}
      footer={null}
    >
      <ToyForm
        initialValues={toy}
        onSubmit={(data) => {
            if (toy.id) {
              onUpdate(toy.id, data);
              onCancel();
            } else {
              console.error("ID игрушки отсутствует!");
            }
        }}
      />
    </Modal>
  );
};
