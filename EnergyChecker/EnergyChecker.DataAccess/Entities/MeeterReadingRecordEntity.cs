namespace EnergyChecker.DataAccess.Entities;

public class MeeterReadingRecordEntity
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public AccountEntity Account { get; set; }
    public DateTime MeeterReadingDateTime { get; set; }
    public int MeterReadValue { get; set; }
    
    public string CompositeKeyString { get; set; } = string.Empty;
}