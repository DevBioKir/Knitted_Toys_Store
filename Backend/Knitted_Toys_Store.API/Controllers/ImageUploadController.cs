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
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(ILogger<ImageUploadController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Загруженный файл не является изображением");

            var imagesFolder = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(imagesFolder, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return Ok(new { filePath = $"/Images/{uniqueFileName}" });
        }

        [HttpGet("check-write-access")]
        public IActionResult CheckWriteAccess()
        {
            var targetDirectory = @"C:\Toys\UploadsToys";

            try
            {
                // Создаем папку, если не существует
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                var testFilePath = Path.Combine(targetDirectory, "test.txt");

                // Пытаемся записать файл
                System.IO.File.WriteAllText(testFilePath, "This is a test.");

                // Удаляем тестовый файл
                System.IO.File.Delete(testFilePath);

                return Ok(new { success = true, message = $"Права на запись в папку `{targetDirectory}` есть." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Ошибка при проверке прав на запись в папку `{targetDirectory}`: {ex.Message}"
                });
            }
        }

        [HttpPost("upload-raw-bytes")]
        public async Task<IActionResult> UploadRawBytes()
        {
            var uploadsFolder = @"C:\Toys\UploadsToys";
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + ".bin";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                await System.IO.File.WriteAllBytesAsync(filePath, ms.ToArray());
            }

            return Ok(new { message = "Успешно загружено в raw-режиме", path = filePath });
        }

        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest("Файл не выбран");

        //        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        //        if (!allowedExtensions.Contains(extension))
        //            return BadRequest("Недопустимый формат файла");

        //        if (!file.ContentType.StartsWith("image/"))
        //            return BadRequest("Файл не является изображением.");

        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFileName = Guid.NewGuid().ToString() + extension;
        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        // Используем MemoryStream для предотвращения падения от ImageSharp
        //        await using var memoryStream = new MemoryStream();
        //        await file.CopyToAsync(memoryStream);
        //        memoryStream.Position = 0;

        //        try
        //        {
        //            using var image = await Image.LoadAsync(memoryStream);
        //        }
        //        catch (SixLabors.ImageSharp.UnknownImageFormatException ex)
        //        {
        //            _logger.LogWarning(ex, "Файл не является допустимым изображением.");
        //            return BadRequest("Файл не является допустимым изображением.");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Ошибка при обработке изображения.");
        //            return StatusCode(500, "Ошибка при обработке изображения.");
        //        }

        //        // Сохраняем изображение
        //        memoryStream.Position = 0;
        //        await using var saveStream = new FileStream(filePath, FileMode.Create);
        //        await memoryStream.CopyToAsync(saveStream);

        //        var imageUrl = $"/Images/{uniqueFileName}";
        //        return Ok(new { imageUrl });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Ошибка при загрузке файла");
        //        return StatusCode(500, "Внутренняя ошибка сервера при загрузке файла");
        //    }
        //}

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
