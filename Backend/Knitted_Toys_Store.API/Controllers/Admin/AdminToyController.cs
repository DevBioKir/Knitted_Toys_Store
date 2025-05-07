using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;
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
        public AdminToyController(IToyService toyService, IWebHostEnvironment env)
        {
            _toyService = toyService;
            _env = env;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ToysResponse>> GetToyByIdAsync(Guid id)
        {
            var toy = await _toyService.GetToyByIdAsync(id);
            if (toy == null)
            {
                return NotFound($"Toy with ID {id} not found.");
            }
            return Ok(toy);
        }

        [HttpGet]
        public async Task<ActionResult<List<ToysResponse>>> GetToysAsync()
        {
            var toys = await _toyService.GetAllToysAsync();

            var responceForToys = toys.Select(t =>
                new ToysResponse(t.Id, t.Name, t.Description, t.Size, t.Price, t.ImageUrl)).ToList();
            return Ok(responceForToys);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Toy?>> UpdateToyAsync(Guid id, [FromBody] ToysRequest request)
        {
            try
            {
                var toy = await _toyService.GetToyByIdAsync(id);
                if (toy == null)
                    return NotFound($"Toy with ID {id} not found.");

                var updateToy = await _toyService.UpdateToyAsync(id, request.Name, request.Description, request.Size, request.Price, request.ImageUrl);
                return Ok(updateToy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the toy: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateToyAsync([FromBody] ToysRequest request)
        {
            try
            {
                var toy = Toy.Create(
                    request.Name,
                    request.Description,
                    request.Size,
                    request.Price,
                    request.ImageUrl);

                var toyId = await _toyService.CreateToyAsync(toy);

                return Ok(toyId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteToyAsync(Guid id)
        {
            var toy = await _toyService.GetToyByIdAsync(id);
            if (toy == null) return NotFound("Игрушка не найдена");

            var imagePath = Path.Combine(_env.WebRootPath, toy.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            await _toyService.DeleteToysAsync(id);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            return Ok(new { message = "Игрушка и её изображение удалены." });

        }
    }
}
