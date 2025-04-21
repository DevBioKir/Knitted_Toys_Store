using Microsoft.AspNetCore.Mvc;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly ILogger<ImageUploadController> _logger;
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(ILogger<ImageUploadController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Файл не выбран");

            if (!image.ContentType.StartsWith("image/"))
                return BadRequest("Загруженный файл не является изображением");

            var imagesFolder = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(imagesFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            };

            return Ok(new { filePath = $"/Images/{uniqueFileName}" });
        }
    }
}
