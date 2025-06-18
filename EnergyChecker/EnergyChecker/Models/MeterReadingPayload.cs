namespace EnergyChecker.Models;

public class MeterReadingPayload
{
    public int Succeeded { get; }
    public int Failed { get; }

    public MeterReadingPayload(int succeeded, int failed)
    {
        Succeeded = succeeded;
        Failed = failed;
    }
}