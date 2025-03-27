namespace Knitted_Toys_Store.Contracts
{
    public record ToysRequest(
        string Name,
        string Description,
        string Size,
        decimal Price,
        string ImageUrl);
}
