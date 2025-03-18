namespace Knitted_Toys_Store.API.Contracts
{
    public record ToysResponce(
        Guid Id,
        string Name,
        string Description,
        string Size,
        decimal Price,
        string ImageUrl); //потом убрать ImageUrl
}
