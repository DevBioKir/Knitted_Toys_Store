using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminToyController : ControllerBase
    {
        private readonly IToyService _toyService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminToyController> _logger;
        public AdminToyController(
            IToyService toyService, 
            IWebHostEnvironment env, 
            ILogger<AdminToyController> logger)
        {
            _toyService = toyService;
            _env = env;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ToysResponse>> GetToyByIdAsync(Guid id)
        {
            _logger.LogInformation("Запрос на получение игрушки с ID: {ToyId}", id);

            var toy = await _toyService.GetToyByIdAsync(id);
            if (toy == null)
            {
                _logger.LogWarning("Игрушка с ID {ToyId} не найдена", id);
                return NotFound($"Toy with ID {id} not found.");
            }

            _logger.LogInformation("Возвращаем игрушку: {ToyName}", toy.Name);
            return Ok(toy);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToysResponse>>> GetAllToysAsync()
        {
            _logger.LogInformation("Запрос на получение всех игрушек");

            var toys = await _toyService.GetAllToysAsync();

            _logger.LogInformation("Возвращаем {Count} игрушек", toys.Count());
            return Ok(toys);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateToyAsync(Guid id, [FromBody] ToysRequest request)
        {
            _logger.LogInformation("Запрос на обновление игрушки с ID: {ToyId}", id);

            var updatedToyId = await _toyService.UpdateToyAsync(id, request);

            _logger.LogInformation("Игрушка обновлена с ID: {ToyId}", updatedToyId);
            return Ok(updatedToyId);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateToyAsync([FromBody] ToysRequest request)
        {
            _logger.LogInformation("Запрос на создание игрушки: {ToyName}", request.Name);

            var toyId = await _toyService.CreateToyAsync(request);

            _logger.LogInformation("Игрушка создана с ID: {ToyId}", toyId);
            return StatusCode(201, new
            {
                id = toyId,
                message = "Игрушка успешно создана",
                location = $"/api/Toy/{toyId}"  // Ручное указание URL
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteToyAsync(Guid id)
        {
            _logger.LogInformation("Запрос на удаление игрушки с ID: {ToyId}", id);

            var deletedToyId = await _toyService.DeleteToyAsync(id);

            _logger.LogInformation("Игрушка удалена с ID: {ToyId}", deletedToyId);
            return Ok(deletedToyId);
        }
    }
}
