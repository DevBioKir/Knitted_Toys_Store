using MapsterMapper;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, ICartService cartService, IMapper mapper)
        {
            _orderService = orderService;
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Order>> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var responceForOrders = _mapper.Map<List<OrderResponse>>(orders);
            return Ok(responceForOrders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrderAsync(
            string surname,
            string name, 
            string phone, 
            string email, 
            string deliveryAddress, 
            string deliveryNotes)
        {
            var cart = await _cartService.GetCurrentCartAsync(HttpContext, Response);
            var order = await _orderService.CreateOrderAsync(cart, surname, name, phone, email,
                    deliveryAddress, deliveryNotes);
            await _cartService.ClearCartAsync(cart.Id);

            //Установим order_id в cookie
            var isDevelopment = HttpContext.Request.Host.Host.Contains("localhost");

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,  // ставим true, чтобы защитить от доступа из JS
                IsEssential = true,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                Secure = !isDevelopment,
                Path = "/"
            };

            Response.Cookies.Append(
                OrderIdentifierMiddleware.OrderCookieName,
                order.Id.ToString(),
                cookieOptions);

            return Ok(order);
        }

        [HttpGet("Current")]
        public async Task<ActionResult<OrderResponse>> GetCurrentOrder()
        {
            var currentOrder = await _orderService.GetCurrentOrderAsync(HttpContext, Response);

            if (currentOrder == null)
            {
                return NotFound("Текущий заказ не найден.");
            }

            var response = _mapper.Map<OrderResponse>(currentOrder);

            return Ok(response);
        }
    }
}
