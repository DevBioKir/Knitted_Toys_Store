import styled from 'styled-components';

interface Props{
    name: string;
    price: number;
}

const Card = styled.div`
  display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
`;

export const CardTitle = ({name, price}: Props) => {
    <div style
}