using EnergyChecker.Models;
using EnergyChecker.Services;
using EnergyChecker.Utils;
using EnergyChecker.Utils.Parsers;
using Microsoft.AspNetCore.Mvc;

namespace EnergyChecker.Controllers;

[ApiController]
[Route("/")]
public class MeterReadingsController : ControllerBase
{
    private readonly IProcessReadingsService _processReadingsService;
    private readonly ParserFactory _parserFactory;

    public MeterReadingsController(IProcessReadingsService processReadingsService, ParserFactory parserFactory)
    {
        _processReadingsService = processReadingsService;
        _parserFactory = parserFactory;
    }

    [HttpPost("meter-reading-uploads")]
    public async Task<MeterReadingPayload> MeeterReadingUploads(IFormFile file, CancellationToken cancellationToken)
    {
        var parser = _parserFactory.CreateCsvParser();

        var stream = new StreamReader(file.OpenReadStream());

        var data = parser.ReadCsvFile(stream, reader =>
        {
            var dateString = reader.GetField<string>("MeterReadingDateTime");
            var date = DateTime.Parse(dateString);
            return new MeterReading
            {
                AccountId = reader.GetField<long>("AccountId"),
                MeeterReadingDateTime = date,
                MeterReadValue = reader.GetField<string>("MeterReadValue"),
            };
        });

        var result = await _processReadingsService.ProcessReadings(data, cancellationToken);

        return new MeterReadingPayload(result.Item1, result.Item2);
    }
}