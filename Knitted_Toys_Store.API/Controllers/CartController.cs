using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

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

            var responceForCarts = _mapper.Map<List<CartResponce>>(carts);
            return Ok(responceForCarts);
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
        [HttpPost]
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

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Cart>> DeleteCartAsync(Guid id)
        {
            return Ok(await _cartService.DeleteCartAsync(id));
        }
    }
}
