using System.Text.RegularExpressions;
using EnergyChecker.DataAccess;
using EnergyChecker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EnergyChecker.Services;

public interface IProcessReadingsService
{
    Task<(int, int)> ProcessReadings(IReadOnlyList<MeterReading> readings, CancellationToken cancellationToken);
}

public class ProcessReadingsService : IProcessReadingsService
{
    private readonly IApplicationDbContext _context;

    public ProcessReadingsService(IApplicationDbContext dbContext)
    {
        _context = dbContext;
    }


    public async Task<(int, int)> ProcessReadings(IReadOnlyList<MeterReading> readings,
        CancellationToken cancellationToken)
    {
        int processed = 0;
        int failed = 0;

        var accountIds = readings.Select(x => x.AccountId).ToArray();

        var accounts = await _context.Accounts.Where(x => accountIds.Contains(x.Id)).ToListAsync(cancellationToken);

        var validRecords = readings.Where(x => Regex.IsMatch(x.MeterReadValue, @"^\d{1,5}$") &&
                                               accounts.Select(x => x.Id).Contains(x.AccountId)
        ).ToList();


        var compositeKeys = validRecords
            .Select(r => MeterReading.BuildKey(r.AccountId, int.Parse(r.MeterReadValue), r.MeeterReadingDateTime))
            .ToList();

        var existingKeys = await _context.MeterRecords
            .Where(r => compositeKeys.Contains(r.CompositeKeyString))
            .Select(r => r.CompositeKeyString)
            .ToListAsync(cancellationToken);

        failed = readings.Count - validRecords.Count + existingKeys.Count;
        processed = validRecords.Count - existingKeys.Count;

        validRecords = validRecords.Where(x =>
            !existingKeys.Contains(MeterReading.BuildKey(x.AccountId, int.Parse(x.MeterReadValue),
                x.MeeterReadingDateTime))).ToList();

        foreach (var meterReading in validRecords)
        {
            try
            {
                await _context.MeterRecords.AddAsync(new MeeterReadingRecordEntity
                {
                    AccountId = meterReading.AccountId,
                    MeterReadValue = int.Parse(meterReading.MeterReadValue),
                    MeeterReadingDateTime = meterReading.MeeterReadingDateTime.ToUniversalTime(),
                    CompositeKeyString = MeterReading.BuildKey(meterReading.AccountId,
                        int.Parse(meterReading.MeterReadValue), meterReading.MeeterReadingDateTime),
                }, cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                failed++;
                processed--;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return (processed, failed);
    }
}

public record MeterReading
{
    public long AccountId { get; set; }
    public DateTime MeeterReadingDateTime { get; set; }
    public string MeterReadValue { get; set; }

    public static string BuildKey(long accountId, int meterValue, DateTime dt) =>
        $"{accountId}_{meterValue}_{dt.ToUniversalTime():yyyyMMddHHmmss}";
}