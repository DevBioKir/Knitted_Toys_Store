using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Policy = "AdminOnly")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public AdminOrderController(IOrderService orderService, IMapper mapper, ICartService cartService)
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

        [HttpGet("GetAllOrdersAsync")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var responceForOrders = _mapper.Map<List<OrderResponse>>(orders);
            return Ok(responceForOrders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrderAsync(string surname,
            string name, string phone, string email, string deliveryAddress, string deliveryNotes)
        {

            var cart = await _cartService.GetCurrentCartAsync(HttpContext, Response);
            var order = await _orderService.CreateOrderAsync(cart, surname, name, phone, email,
                    deliveryAddress, deliveryNotes);
            await _cartService.ClearCartAsync(cart.Id);

            return Ok(order);
        }

        [HttpPut]
        public async Task<ActionResult<OrderStatus>> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            return Ok(await _orderService.UpdateOrderStatusAsync(orderId, newStatus));
        }

        [HttpDelete]
        public async Task<ActionResult<OrderResponse>> DeleteOrderAsync(Guid id)
        {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null) return NotFound($"Order with ID {id} not found.");

                return Ok(await _orderService.DeleteOrderAsync(id));
        }

        [HttpGet("GetOrderCountAsync")]
        public async Task<ActionResult<int>> GetOrderCountAsync()
        {
            return Ok(await _orderService.GetOrderCountAsync());
        }

        [HttpGet("GetTotalRevenueAsync")]
        public async Task<ActionResult<decimal>> GetTotalRevenueAsync()
        {
            return Ok(await _orderService.GetTotalRevenueAsync());
        }

        [HttpGet("SearchOrderAsync")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> SearchOrderAsync(
            string? surnameCustomer = null,
            string? nameCustomer = null,
            string? phoneNumber = null,
            string? email = null,
            string? deliveryAddress = null,
            OrderStatus? status = null)
        {
            return Ok(await _orderService.SearchOrderAsync(
                surnameCustomer,
                nameCustomer,
                phoneNumber,
                email,
                deliveryAddress,
                status));
        }

        [HttpGet("GetOrderByStatusAsync")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrderByStatusAsync(OrderStatus newStatus)
        {
            return Ok(await _orderService.GetOrderByStatusAsync(newStatus));
        }

        [HttpPost("clone-to-cart")]
        public async Task<ActionResult<OrderResponse>> CloneOrderToCartAsync(Guid orderId)
        {
            try
            {
                var cart = await _orderService.CloneOrderToCartAsync(orderId);
                var cartResponse = _mapper.Map<CartResponse>(cart);
                return Ok(cartResponse);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
