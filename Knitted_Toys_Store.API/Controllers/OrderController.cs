using AutoMapper;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
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
        public async Task<ActionResult<List<OrderResponce>>> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var responceForOrders = _mapper.Map<List<OrderResponce>>(orders);
            return Ok(responceForOrders);
        }

        //[HttpPost]
        //public async Task<ActionResult<OrderResponce>> CreateOrderAsync(Cart cart, string surname, 
        //    string name, string phone, string email, string deliveryAddress, string deliveryNotes)
        //{
        //    try
        //    {
        //        var cartId = 
        //        var order = await _orderService.CreateOrderAsync(cart, surname, name, phone, email, 
        //            deliveryAddress, deliveryNotes);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}
