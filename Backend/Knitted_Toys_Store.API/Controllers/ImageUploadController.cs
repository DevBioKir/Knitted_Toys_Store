using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageUploadController> _logger;

        public ImageUploadController(IWebHostEnvironment env, ILogger<ImageUploadController> logger)
        {
            _env = env;
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

                // Определяем путь для сохранения
                string uploadsFolder;
                if (string.IsNullOrEmpty(_env.WebRootPath))
                {
                    // Для API проектов без wwwroot
                    uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                }
                else
                {
                    uploadsFolder = Path.Combine(_env.WebRootPath, "Images");
                }

                _logger.LogInformation($"Путь для сохранения: {uploadsFolder}");

                if (!Directory.Exists(uploadsFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        _logger.LogInformation($"Создана директория: {uploadsFolder}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при создании директории");
                        return StatusCode(500, "Не удалось создать директорию для сохранения файлов");
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                _logger.LogInformation($"Сохранение файла по пути: {filePath}");

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
