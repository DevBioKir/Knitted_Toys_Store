namespace Knitted_Toys_Store.Contracts
{
    public record ToysResponse(
        Guid Id,
        string Name,
        string Description,
        string Size,
        decimal Price,
        string ImageUrl); //потом убрать ImageUrl
}
