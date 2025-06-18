using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace EnergyChecker.Utils.Parsers;

public interface ICsvParser
{
    public IReadOnlyList<T> ReadCsvFile<T>(StreamReader reader, Func<CsvReader, T> f);
}

public class CsvParser : ICsvParser
{
    public IReadOnlyList<T> ReadCsvFile<T>(StreamReader reader, Func<CsvReader, T> f)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
        };
        using var csv = new CsvReader(reader, config);
        var records = new List<T>();
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var record = f(csv);
            records.Add(record);
        }

        return records;
    }
}