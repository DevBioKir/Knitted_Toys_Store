namespace Knitted_Toys_Store.API.Contracts
{
    public record ToysRequest(
        string Name,
        string Description,
        string Size,
        decimal Price,
        string ImageUrl);
}
