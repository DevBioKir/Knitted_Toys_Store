using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Knitted_Toys_Store.API.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class ToysPriceUpdateController : ControllerBase
    {
        private readonly IToyService _toyService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadToysFromExcel> _logger;

        public ToysPriceUpdateController(
            ILogger<UploadToysFromExcel> logger,
            IWebHostEnvironment env, IToyService toyService)
        {
            _toyService = toyService;
            _logger = logger;
            _env = env;
        }

        [HttpGet("ExportToysToExcel")]
        public async Task<ActionResult> ExportToysToExcel()
        {
            var toys = await _toyService.GetAllToysAsync();

            using var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("Toys");

            workSheet.Cell(1, 1).Value = "Id";
            workSheet.Cell(1, 2).Value = "Name";
            workSheet.Cell(1, 3).Value = "Price";

            int row = 2;
            foreach (var toy in toys)
            {
                if (toy == null) continue;

                workSheet.Cell(row, 1).Value = toy.Id.ToString();
                workSheet.Cell(row, 2).Value = toy.Name ?? "";
                workSheet.Cell(row, 3).Value = toy.Price;
                row++;
            }

            // поток в памяти
            using var stream = new MemoryStream();
            // записываем файл в поток
            workBook.SaveAs(stream);
            // переход в начальную позицию потока, для отправки файла пользователю через чтение
            stream.Position = 0;

            string fileName = $"Toys_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            // File() ожидает массив байтов, поэтому, To.Array(). MIME Тип для Excel файлов
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("ImportPricesFromExcel")]
        public async Task<ActionResult> ImportPricesFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest("Excel-файл не загружен");

            if (!excelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Поддерживается только формат .xlsx");

            // создаём временную папку
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            var filePath = Path.Combine(tempFolder, excelFile.FileName);

            // сохраняем Excel-файл во временную папку
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await excelFile.CopyToAsync(stream);
            }

            int updatedCount = 0;
            var updatedIds = new List<Guid>();

            try
            {
                using var workbook = new XLWorkbook(filePath);
                var workSheet = workbook.Worksheet(1);
                var rows = workSheet.RowsUsed().Skip(1); // пропускаем заголовок

                foreach (var row in rows)
                {
                    try
                    {
                        var idToy = row.Cell(1).GetString().Trim();
                        var name = row.Cell(1).GetString();
                        var price = row.Cell(3).GetValue<decimal>();

                        if (!Guid.TryParse(idToy, out var id))
                        {
                            _logger.LogWarning("Строка пропущена — некорректный Id или Price");
                            continue;
                        }

                        var toy = await _toyService.GetToyByIdAsync(id);
                        if (toy == null)
                        {
                            _logger.LogWarning("Игрушка с Id {ToyId} не найдена", id);
                            continue;
                        }

                        var updateToy = new ToysRequest(
                            toy.Name,
                            toy.Description,
                            toy.Size,
                            price,
                            toy.ImageUrl);

                        await _toyService.UpdateToyAsync(id, updateToy);

                        updatedCount++;
                        updatedIds.Add(id);
                    }
                    catch (Exception exRow)
                    {
                        _logger.LogError(exRow, "Ошибка при обработке строки Excel");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке Excel-файла");
                return StatusCode(500, "Ошибка при обработке файла");
            }
            finally
            {
                // Удаляем временные файлы
                Directory.Delete(tempFolder, true);
            }

            return Ok(new
            {
                Message = $"Обновлено {updatedCount} игрушек",
                UpdatedIds = updatedIds
            });
        }
    }
}
