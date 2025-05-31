import styled from 'styled-components';

interface Props{
    name: string;
    price: number;
    size: string;
}

const Card = styled.div`
  display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
`;

export const CardTitle = ({name, price, size}: Props) => {
    return(
    <div>
        <p className="card_name">{name}</p>
        <p className="card_price">{price}</p>
        <p className="card_size">{size}</p>
    </div>
    );
};