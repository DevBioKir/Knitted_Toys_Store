using AutoMapper;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Policy = "AdminOnly")]
    public class AdminCartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartIdentifierMiddleware> _logger;
        private readonly IMapper _mapper;
        public AdminCartController(ICartService cartService, ILogger<CartIdentifierMiddleware> logger, IMapper mapper)
        {
            _cartService = cartService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CartResponse>> GetCartByIdAsync(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            return Ok(cart);
        }

        [HttpGet("Current")]
        public async Task<ActionResult<CartResponse>> GetCurrentCart()
        {
            var cartCurrent = await _cartService.GetCurrentCartAsync(HttpContext, Response);
            var responceForCarts = _mapper.Map<CartResponse>(cartCurrent);

            return Ok(responceForCarts);
        }

        [HttpGet("GetAllCartsAsyn")]
        public async Task<ActionResult<List<CartResponse>>> GetAllCartsAsyn()
        {
            var carts = await _cartService.GetAllCarts();

            var responceForCarts = _mapper.Map<List<CartResponse>>(carts);
            return Ok(responceForCarts);
        }

        [HttpPut("UpdateCartAsync")]
        public async Task<ActionResult<CartResponse>> UpdateCartAsync(Guid cartId, [FromBody] CartRequest request)
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

                var response = _mapper.Map<CartResponse>(updatedCart);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении корзины с ID {CartId}: {Message}", cartId, ex.Message);
                return StatusCode(500, $"An error occurred while updating the cart: {ex.Message}");
            }
        }

        [HttpPut("UpdateItemFromCartAsync")]
        public async Task<ActionResult<CartResponse>> UpdateItemFromCartAsync(Guid cartId, [FromBody] CartRequest request)
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
                    cart.SetItemQuantity(item.ToyId, item.Quantity);
                }

                await _cartService.UpdateAsync(cart);

                // Обновленный товар в корзине
                var updatedCart = await _cartService.GetCartByIdAsync(cartId);

                var response = _mapper.Map<CartResponse>(updatedCart);

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
        public async Task<ActionResult<CartItemsResponse>> AddToCartAsync(Guid cartId, Guid toyId,
            int quantity)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(cartId);

                if (cart == null) return NotFound($"Cart with ID {cartId} not found.");

                var cartItem = await _cartService.AddToCartAsync(cartId, toyId, quantity);

                return Ok(cartItem);
            }
            catch (Exception err)
            {
                return BadRequest(err.ToString());
            }
        }

        [HttpDelete("RemoveItemFromCart")] //удаление позиции в корзине полностью
        public async Task<ActionResult<Cart>> RemoveItemFromCart(Guid cartId, Guid toyId)
        {
            var cart = await _cartService.GetCartByIdAsync(cartId);
            if (cart == null) return NotFound($"Cart with ID {cartId} not found.");
      
            return Ok(await _cartService.RemoveItemFromCartAsync(cartId, toyId));
        }

        [HttpDelete("ReduceQuantityItemAsync")] //уменьшить количество товара в позиции
        public async Task<ActionResult<Cart>> ReduceQuantityItemAsync(Guid cartId, Guid toyId)
        {
            var cart = await _cartService.GetCartByIdAsync(cartId);
            if (cart == null) return NotFound($"Cart with ID {cartId} not found.");

            return Ok(await _cartService.ReduceQuantityItemAsync(cartId, toyId));
        }

        [HttpDelete("DeleteCartAsync")] 
        public async Task<ActionResult<Cart>> DeleteCartAsync(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null) return NotFound($"Cart with ID {id} not found.");

            return Ok(await _cartService.DeleteCartAsync(id));
        }
    }
}
