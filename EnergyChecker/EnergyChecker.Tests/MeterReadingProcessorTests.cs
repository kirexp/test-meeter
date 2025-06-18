using AutoFixture;
using EnergyChecker.DataAccess;
using EnergyChecker.DataAccess.Entities;
using EnergyChecker.Services;
using Microsoft.EntityFrameworkCore;

namespace EnergyChecker.Tests;

public class MeterReadingProcessorTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task ProcessReadings_ShouldInsertValidReading()
    {
        int accountId = 1;
        var context = TestDbContextFactory.Create();
        context.Accounts.Add(GenerateAccountEntity(accountId));
        await context.SaveChangesAsync();

        var sut = new ProcessReadingsService(context);

        var reading = new MeterReading
        {
            AccountId = accountId,
            MeterReadValue = "123",
            MeeterReadingDateTime = new DateTime(2024, 01, 01, 10, 0, 0)
        };

        var (processed, failed) = await sut.ProcessReadings(new[] { reading }, CancellationToken.None);

        Assert.Equal(1, processed);
        Assert.Equal(0, failed);
        Assert.Single(context.MeterRecords);
    }

    [Fact]
    public async Task ProcessReadings_ShouldFailInvalidMeterValue()
    {
        var context = TestDbContextFactory.Create();
        var accountId = 1;
        context.Accounts.Add(GenerateAccountEntity(accountId));
        await context.SaveChangesAsync();

        var sut = new ProcessReadingsService(context);

        var reading = new MeterReading
        {
            AccountId = accountId,
            MeterReadValue = "ABCDE",
            MeeterReadingDateTime = DateTime.Now
        };

        var (processed, failed) = await sut.ProcessReadings(new[] { reading }, CancellationToken.None);

        Assert.Equal(0, processed);
        Assert.Equal(1, failed);
    }

    [Fact]
    public async Task ProcessReadings_ShouldFailUnknownAccount()
    {
        var context = TestDbContextFactory.Create();
        var sut = new ProcessReadingsService(context);

        var reading = new MeterReading
        {
            AccountId = 42,
            MeterReadValue = "100",
            MeeterReadingDateTime = DateTime.Now
        };

        var (processed, failed) = await sut.ProcessReadings(new[] { reading }, CancellationToken.None);

        Assert.Equal(0, processed);
        Assert.Equal(1, failed);
    }

    [Fact]
    public async Task ProcessReadings_ShouldSkipDuplicateCompositeKey()
    {
        var context = TestDbContextFactory.Create();
        var accountId = 1;
        var value = 200;
        var dt = new DateTime(2024, 06, 18, 9, 24, 0);

        context.Accounts.Add(GenerateAccountEntity(accountId));
        context.MeterRecords.Add(new MeeterReadingRecordEntity
        {
            AccountId = accountId,
            MeterReadValue = value,
            MeeterReadingDateTime = dt,
            CompositeKeyString = MeterReading.BuildKey(accountId, value, dt)
        });
        await context.SaveChangesAsync();

        var sut = new ProcessReadingsService(context);

        var duplicate = new MeterReading
        {
            AccountId = accountId,
            MeterReadValue = value.ToString(),
            MeeterReadingDateTime = dt
        };

        var (processed, failed) = await sut.ProcessReadings(new[] { duplicate }, CancellationToken.None);

        Assert.Equal(0, processed);
        Assert.Equal(1, failed);
    }

    private AccountEntity GenerateAccountEntity(long accountId)
    {
        return _fixture.Build<AccountEntity>()
            .With(x => x.Id, accountId)
            .Create();
    }

    private static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}