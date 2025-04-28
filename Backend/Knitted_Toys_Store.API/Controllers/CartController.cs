using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.App.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartIdentifierMiddleware> _logger;
        private readonly IMapper _mapper;
        public CartController(ICartService cartService, ILogger<CartIdentifierMiddleware> logger, IMapper mapper)
        {
            _cartService = cartService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("Current")]
        public async Task<ActionResult<CartResponce>> GetCurrentCart()
        {
            var cartCurrent = await _cartService.GetCurrentCartAsync(HttpContext, Response);
            var responceForCarts = _mapper.Map<CartResponce>(cartCurrent);

            return Ok(responceForCarts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CartResponce>> GetCartByIdAsync(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            var responceForCart = _mapper.Map<CartResponce>(cart);
            return Ok(responceForCart);
        }

        [HttpPut]
        public async Task<ActionResult<CartResponce>> UpdateCartAsync(Guid cartId, [FromBody] CartRequest request)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(cartId);
                if (cart == null)
                    return NotFound($"Cart with ID {cartId} not found.");

                // Проверяем, что версия RowVersion совпадает
                if (!cart.RowVersion.SequenceEqual(request.RowVersion))
                {
                    return Conflict("The cart data has been modified by another process.");
                }

                foreach (var item in request.CartItemsRequest)
                {
                    cart.UpdateItemQuantity(item.ToyId, item.Quantity);
                }

                await _cartService.UpdateAsync(cart);

                // Обновленный товар в корзине
                var updatedCart = await _cartService.GetCartByIdAsync(cartId);

                var response = _mapper.Map<CartResponce>(updatedCart);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the cart: {ex.Message}");
            }
        }

        [HttpPost("AddToys")]
        public async Task<ActionResult<CartItemsResponce>> AddToCartAsync(Guid cartId, Guid toyId, 
            int quantity)
        {
                var cart = await _cartService.GetCartByIdAsync(cartId);
                Console.WriteLine(cart.RowVersion);

                if (cart == null) return NotFound($"Cart with ID {cartId} not found.");

                var cartItem = await _cartService.AddToCartAsync(cartId, toyId, quantity);

                return Ok(cartItem);
        }
    }
}
