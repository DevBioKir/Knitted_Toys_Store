
namespace Knitted_Toys_Store.Domain.Models.Domain
{
    public class Toy
    {
        public const int MAX_LENGTH_NAME = 200;
        public const int MAX_LENGTH_DESCRIPTION = 3000;
        public const int MAX_LENGTH_SIZE = 20;

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Size { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;//путь к изображению

        public List<OrderItems> OrderItems { get; } = []; //у одной игрушки может быть много позиций в заказе
        public List<CartItems> CartItems { get; } = []; //у одной игрушки может быть много позиций в корзине

        private Toy() { }
        public static Toy Create(
            string name, string description, string size, decimal price, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_LENGTH_NAME)
            {
                throw new ArgumentException("Name toy must not be empty and should not exceed 200 characters.");
            }
            if (string.IsNullOrWhiteSpace(name) || description.Length > MAX_LENGTH_DESCRIPTION)
            {
                throw new ArgumentException("Description should not exceed 3000 characters.");
            }
            if (string.IsNullOrWhiteSpace(size) || size.Length > MAX_LENGTH_SIZE)
            {
                throw new ArgumentException("Size must not be empty and should not exceed 20 characters.");
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL must not be empty.");
            }
            return new Toy
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Size = size,
                Price = price,
                ImageUrl = imageUrl
            };
        }
    }
}
