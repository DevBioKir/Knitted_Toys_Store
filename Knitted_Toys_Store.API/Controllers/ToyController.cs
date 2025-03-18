using Knitted_Toys_Store.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToyController : ControllerBase
    {
        private readonly IToyService
        public ToyController(IToyService toyService)
        {
            
        }
    }
}
