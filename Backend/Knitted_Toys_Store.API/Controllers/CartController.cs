using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.VisualBasic;
using Knitted_Toys_Store.API.Middleware;
using Knitted_Toys_Store.API.Helpers;

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

        private Guid? GetCartIdFromContext()
        {
            if (HttpContext.Items.TryGetValue(CartIdentifierMiddleware.CartCookieName, out var cartIdObj)
                && cartIdObj is Guid cartId)
            {
                return cartId;
            }

            return null;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CartResponce>> GetCartByIdAsync(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            return Ok(cart);
        }

        [HttpGet("Current")]
        public async Task<ActionResult<CartResponce>> GetCurrentCart()
        {
            _logger.LogInformation("Получен запрос на текущую корзину");

            // Проверяем, есть ли ID корзины в Items (установлен middleware)
            if (HttpContext.Items.TryGetValue(CartIdentifierMiddleware.CartCookieName, out var cartIdObj) &&
                cartIdObj is Guid cartGuid)
            {
                _logger.LogInformation($"Найден ID корзины в HttpContext.Items: {cartGuid}");

                // Корзина уже проверена middleware, просто получаем её
                var existingCart = await _cartService.GetCartByIdAsync(cartGuid);
                if (existingCart != null)
                {
                    _logger.LogInformation($"Найдена существующая корзина с ID: {cartGuid}");
                    return Ok(existingCart);
                }
            }

            // Проверяем наличие cookie
            var cartIdCookie = Request.Cookies[CartIdentifierMiddleware.CartCookieName];
            if (cartIdCookie != null)
            {
                _logger.LogInformation($"Найден ID корзины в cookie: {cartIdCookie}");

                // Пытаемся преобразовать строку в Guid
                if (Guid.TryParse(cartIdCookie, out Guid cookieCartGuid))
                {
                    // Проверяем, существует ли корзина с таким ID
                    var existingCart = await _cartService.GetCartByIdAsync(cookieCartGuid);
                    if (existingCart != null)
                    {
                        _logger.LogInformation($"Найдена существующая корзина с ID: {cookieCartGuid}");
                        return Ok(existingCart);
                    }
                    else
                    {
                        _logger.LogWarning($"Корзина с ID {cookieCartGuid} не найдена в базе данных");
                    }
                }
                else
                {
                    _logger.LogWarning($"Не удалось преобразовать ID корзины из cookie '{cartIdCookie}' в Guid");
                }
            }
            else
            {
                _logger.LogInformation("ID корзины не найден в cookie");
            }

            // Создаем новую корзину
            _logger.LogInformation("Создаем новую корзину");
            var newCart = await _cartService.CreateCartAsync();

            // Устанавливаем cookie
            Response.Cookies.Append(CartIdentifierMiddleware.CartCookieName, newCart.Id.ToString(), new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTimeOffset.Now.AddDays(30),
                Path = "/"
            });

            _logger.LogInformation($"Создана новая корзина с ID: {newCart.Id}");
            return Ok(newCart);
        }



        //[HttpGet("Current")]
        //public async Task<ActionResult<CartResponce>> GetCurrentCart()
        //{
        //    // Логируем информацию о запросе
        //    _logger.LogInformation("Получен запрос на текущую корзину");

        //    // Проверяем наличие cookie
        //    var cartIdCookie = Request.Cookies["cart_id"];
        //    if (cartIdCookie != null)
        //    {
        //        _logger.LogInformation($"Найден ID корзины в cookie: {cartIdCookie}");

        //        if (Guid.TryParse(cartIdCookie, out Guid cartGuid))
        //        {
        //            // Проверяем, существует ли корзина с таким ID
        //            var existingCart = await _cartService.GetCartByIdAsync(cartGuid);
        //            if (existingCart != null)
        //            {
        //                _logger.LogInformation($"Найдена существующая корзина с ID: {cartIdCookie}");
        //                return Ok(existingCart);
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"Корзина с ID {cartIdCookie} не найдена в базе данных");
        //            }
        //        }
        //        else
        //        {
        //            _logger.LogInformation($"не удалось преобразовать ID корзины из cookie {cartIdCookie} в Guid");
        //        }
        //    }
        //    else
        //    {
        //        _logger.LogInformation("ID корзины не найден в cookie");
        //    }

        //    // Создаем новую корзину
        //    _logger.LogInformation("Создаем новую корзину");
        //    var newCart = await _cartService.CreateCartAsync();

        //    // Устанавливаем cookie
        //    Response.Cookies.Append("cart_id", newCart.Id.ToString(), new CookieOptions
        //    {
        //        HttpOnly = false,
        //        Expires = DateTimeOffset.Now.AddDays(30)
        //    });

        //    _logger.LogInformation($"Создана новая корзина с ID: {newCart.Id}");
        //    return Ok(newCart);
        //}

        [HttpGet]
        public async Task<ActionResult<List<CartResponce>>> GetAllCartsAsyn()
        {
            var carts = await _cartService.GetAllCarts();

            var responceForCarts = _mapper.Map<List<CartResponce>>(carts);
            return Ok(responceForCarts);
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
