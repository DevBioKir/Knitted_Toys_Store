namespace Knitted_Toys_Store.API.Contracts
{
    public record CartResponce(
        Guid Id,
        DateTime CreateAt,
        DateTime LastUpdate,
        decimal TotalAmount);
}
