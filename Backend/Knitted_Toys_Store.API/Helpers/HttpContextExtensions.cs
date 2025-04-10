namespace Knitted_Toys_Store.API.Helpers
{
    public static class HttpContextExtensions
    {
        public static Guid GetCardId(this HttpContext context)
        {
            const string CartKey = "cart_id";

            if (context.Items.TryGetValue(CartKey, out var value) && value is Guid cartId)
            {
                return cartId;
            }

            throw new InvalidOperationException("Cart ID is not found in HttpContext.");

        }
    }
}
