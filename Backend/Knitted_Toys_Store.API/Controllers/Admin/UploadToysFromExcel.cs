using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class UploadToysFromExcel : ControllerBase
    {
        private readonly IToyService _toyService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadToysFromExcel> _logger;

        public UploadToysFromExcel(
            ILogger<UploadToysFromExcel> logger, 
            IWebHostEnvironment env, IToyService toyService)
        {
            _toyService = toyService;
            _logger = logger;
            _env = env;
        }

        [HttpPost("UploadExel")]
        public async Task<ActionResult> UploadToysFromExel(IFormFile zipFile)
        {
            if (zipFile == null || zipFile.Length == 0) //проверяем загрузку zip файла
                return BadRequest("Zip file not loaded");

            if (!zipFile.FileName.EndsWith(".zip")) //проверка на тип файла
                return BadRequest("Only ZIP archive is supported");

            //создаем временную папку с уникальным именем
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); 

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder); //если не существует указанного пути, создать

            var zipPath = Path.Combine(tempFolder, zipFile.Name);
            // Копируем zip-файл в временную директорию
            await using (var stream = new FileStream(zipPath, FileMode.Create))
            {
                await zipFile.CopyToAsync(stream);
            }

            //распаковываем zip во временную папку
            ZipFile.ExtractToDirectory(zipPath, tempFolder);

            // Логируем содержимое распакованной папки
            var allFiles = Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories);
            _logger.LogInformation("Найденные файлы после распаковки ZIP:");
            foreach (var file in allFiles)
            {
                _logger.LogInformation(file);
            }

            //поиск excel файла в распакованной папке
            var excelFile = Directory.GetFiles(tempFolder, "*.xlsx", SearchOption.AllDirectories).FirstOrDefault();

            if (excelFile == null)
            {
                _logger.LogWarning("Excel-файл не найден в ZIP-архиве");
                return BadRequest("Excel file not founded in zip-folder");
            } 

            var toysCount = 0;
            //указание пути для картинок
            var imagesFolder = Path.Combine(_env.WebRootPath, "Images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder); //если не существует указанного пути, создать

            using (var workbook = new XLWorkbook(excelFile))
            {
                var worksheet = workbook.Worksheets.First();
                var rows = worksheet.RowsUsed().Skip(1); // Пропускаем заголовок

                foreach (var row in rows)
                {
                    try
                    {
                        var name = row.Cell(1).GetString();
                        var description = row.Cell(2).GetString();
                        var size = row.Cell(3).GetString();
                        var price = row.Cell(4).GetValue<decimal>();
                        var imageFileName = row.Cell(5).GetString();

                        //путь к картинке
                        var sourceImagePath = Path.Combine(tempFolder, imageFileName);
                        if (!System.IO.File.Exists(sourceImagePath))
                        {
                            _logger.LogWarning($"Изображение не найдено: {sourceImagePath}");
                            continue;
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{imageFileName}";
                        var destImagePath = Path.Combine(imagesFolder, uniqueFileName);
                        System.IO.File.Copy(sourceImagePath, destImagePath, true);

                        var imageUrl = $"/Images/{uniqueFileName}".Replace("\\", "/");

                        var toy = Toy.Create(name, description, size, price, imageUrl);
                        await _toyService.CreateToyAsync(toy);
                        toysCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при чтении строки Excel");
                    }
                }
            }

            // Удаляем временные файлы
            Directory.Delete(tempFolder, true);

            return Ok(new { added = toysCount });
        }
    }
}
