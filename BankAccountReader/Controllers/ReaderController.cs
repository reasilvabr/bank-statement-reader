using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BankAccountReader.Domain;
using BankAccountReader.Service;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountReader.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReaderController : ControllerBase
{
    private readonly IStatementReaderService _service;

    public ReaderController(IStatementReaderService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index(IFormFile pdfFile, [FromQuery] int bank, [FromQuery] string format = "json", [FromQuery] string culture = "pt-BR")
    {
        var text = new StringBuilder();
        using (var reader = new PdfReader(pdfFile.OpenReadStream()))
        {
            using (var doc = new PdfDocument(reader))
            {
                for (int page = 1; page <= doc.GetNumberOfPages(); page++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(doc.GetPage(page)));
                }
            }
        }

        var result = _service.ReadStatement(text.ToString(), (BankCode)bank);

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(_service.ConvertToCsv(result));
        }
        return Ok(result);
    }
}