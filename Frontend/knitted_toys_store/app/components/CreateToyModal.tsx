import { Modal } from "antd";
import { ToyRequest } from "../types/Toy/ToyRequest";
import { ToyForm } from "./ToyForm";
import { data } from "framer-motion/client";

interface Prop {
  isOpen: boolean;
  onCancel: () => void;
  onCreate: (toy: ToyRequest) => void;
}

export const CreateToyModal = ({ isOpen, onCancel, onCreate }: Prop) => {
  return (
    <Modal
      title="Добавить игрушку"
      open={isOpen}
      onCancel={onCancel}
      footer={null}
    >
      <ToyForm
        onSubmit={(data) => {
          onCreate(data);
          onCancel();
        }}/>
    </Modal>
  );
};
