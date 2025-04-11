namespace Knitted_Toys_Store.Contracts.Contracts
{
    public record CartUpdateRequest
    {
        public List<CartItemsRequest> Items { get; set; } = new();
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
