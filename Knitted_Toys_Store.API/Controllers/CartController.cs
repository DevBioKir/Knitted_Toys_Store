using Knitted_Toys_Store.API.Contracts;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Toy>> GetCartByIdAsync(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            return Ok(cart);
        }

        [HttpGet]
        public async Task<ActionResult<List<CartResponce>>> GetAllCartsAsyn()
        {
            var carts = await _cartService.GetAllCarts();

            var responceForCarts = carts.Select(c =>
                new CartResponce(c.Id, c.CreateAt, c.LastUpdate, c.TotalAmount));
            return Ok(carts);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CartResponce>> UpdateCartAsync(Guid cartId, Guid toyId, int quantity)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(cartId);
                if (cart == null)
                    return NotFound($"Cart with ID {cartId} not found.");

                cart.UpdateItemQuantity(toyId, quantity);
                await _cartService.UpdateAsync(cart);

                var updateCar = await _cartService.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the cart: {ex.Message}");
            }
        }
    }
}
