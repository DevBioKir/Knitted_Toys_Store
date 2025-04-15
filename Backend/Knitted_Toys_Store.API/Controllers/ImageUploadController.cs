using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SixLabors.ImageSharp;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly ILogger<ImageUploadController> _logger;

        public ImageUploadController(ILogger<ImageUploadController> logger)
        {
            _logger = logger;
        }

        [HttpPost("UploadImage")]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB
        [RequestSizeLimit(10485760)] // 10MB
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Файл не выбран");

                _logger.LogInformation($"Получен файл: {file.FileName}, размер: {file.Length} байт");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Недопустимый формат файла");

                // Путь для загрузки файла
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation($"Создана директория: {uploadsFolder}");
                }

                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                _logger.LogInformation($"Сохранение файла по пути: {filePath}");

                // Открытие и проверка файла как изображения с помощью ImageSharp
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // Можно добавить дополнительные проверки, например, разрешение изображения или его размер
                    _logger.LogInformation($"Изображение {file.FileName} успешно загружено.");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/Images/{uniqueFileName}";
                _logger.LogInformation($"Файл успешно сохранен, URL: {imageUrl}");

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файла");
                return StatusCode(500, "Внутренняя ошибка сервера при загрузке файла");
            }
        }

        [HttpGet("force-crash")]
        public IActionResult ForceCrash()
        {
            try
            {
                throw new Exception("Искусственный краш для теста логирования");
            }
            catch (Exception ex)
            {
                // Логируем исключение через Serilog
                Log.Error(ex, "Произошла ошибка при искусственном краше");
                return StatusCode(500, "Произошла ошибка. Посмотрите логи для деталей.");
            }
        }
    }


    //[ApiController]
    //[Route("[controller]")]
    //public class ImageUploadController : ControllerBase
    //{
    //    private readonly IWebHostEnvironment _env;

    //    public ImageUploadController(IWebHostEnvironment env)
    //    {
    //        _env = env;
    //    }

    //    [HttpPost("UploadImage")]
    //    public async Task<IActionResult> UploadImage(IFormFile file)
    //    {
    //        if (file == null || file.Length == 0)
    //            return BadRequest("Файл не выбран");

    //        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    //        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

    //        if (!allowedExtensions.Contains(extension))
    //            return BadRequest("Недопустимый формат файла");

    //        var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "Images");

    //        if (!Directory.Exists(uploadsFolder))
    //        {
    //            Directory.CreateDirectory(uploadsFolder);
    //        }

    //        var uniqueFileName = Guid.NewGuid() + extension;
    //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        var imageUrl = $"/Images/{uniqueFileName}";
    //        return Ok(new { imageUrl });
    //    }
    //}
}
