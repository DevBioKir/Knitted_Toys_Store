using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("controller")]
    [Authorize(Policy = "AdminOnly")]
    public class CreateReportOrders
    {
        private readonly ILogger<CreateReportOrders> _logger;

        public CreateReportOrders(ILogger<CreateReportOrders> logger)
        {
            _logger = logger;
        }


        //public Task<ActionResult> CreateReport(IFormFile reportFile)
        //{

        //}

    }
}
