using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knitted_Toys.Domain.Models.Domain
{
    public class Toy
    {
        const int MAX_LENGTH_NAME = 200;
        const int MAX_LENGTH_DESCRIPTION = 3000;
        const int MAX_LENGTH_SIZE = 20;

        private Toy(string name, string description, string size, decimal price, string imageUrl)
        {
            Id = Guid.NewGuid(); // Генерируем новый Id внутри конструктора
            Name = name;
            Description = description;
            Size = size;
            Price = price;
            ImageUrl = imageUrl;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;//путь к изображению

        public List<OrderItems> OrderItems { get; set; } = []; //у одной игрушки может быть много позиций в заказе
        public List<CartItems> CartItems { get; set; } = []; //у одной игрушки может быть много позиций в корзине

        public (Toy Toy, string Error) CreateToy(string name, string description, string size, decimal price, string imageUrl)
        {
            var error = string.Empty;

            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_LENGTH_NAME)
            {
                error = "Name toy must not be empty and should not exceed 200 characters.";
                return (null, error);
            }
            if (description.Length > MAX_LENGTH_DESCRIPTION)
            {
                error = "Description should not exceed 3000 characters.";
                return (null, error);
            }
            if (string.IsNullOrWhiteSpace(size) || size.Length > MAX_LENGTH_SIZE)
            {
                error = "Size must not be empty and should not exceed 50 characters.";
                return (null, error);
            }
            if (price <= 0)
            {
                error = "Price must be a positive value.";
                return (null, error);
            }
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                error = "Image URL must not be empty.";
            }

            var toy = new Toy(name, description, size, price, imageUrl);

            return (toy, error);
        }
    }
}
