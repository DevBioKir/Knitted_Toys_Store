using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.VisualBasic;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Cart>> GetCartByIdAsync(Guid id)
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

            var responceForCarts = _mapper.Map<List<CartResponce>>(carts);
            return Ok(responceForCarts);
        }

        [HttpPut("{id:guid}")]
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

        [HttpPost("CreateCart")]
        public async Task<ActionResult<Guid>> CreateCartAsync()
        {
            try
            {
                var cart = await _cartService.CreateCartAsync();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
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

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Cart>> DeleteCartAsync(Guid id)
        {
            return Ok(await _cartService.DeleteCartAsync(id));
        }
    }
}
