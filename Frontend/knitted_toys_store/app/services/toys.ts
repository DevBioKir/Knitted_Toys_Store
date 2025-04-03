import { body } from "framer-motion/client";
import { headers } from "next/headers";

export interface ToyRequest{
    name: string;
    description: string;
    size: string;
    price: number;
    imageUrl: string;
}

export const getAllToys = async() => {
    const responce = await fetch("http://localhost:5237/Toy");

    return responce.json();
};

export const createBook = async(toyrequest: ToyRequest) => {
    await fetch("http://localhost:5237/Toy", {
        method: "POST",
        headers: {
            "content-type": "applicaton/json",
        },
        body: JSON.stringify(toyrequest), 
    });
};

//export const updateToy