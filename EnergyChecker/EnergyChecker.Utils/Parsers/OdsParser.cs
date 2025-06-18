using System.Data;
using Zaretto.ODS;

namespace EnergyChecker.Utils.Parsers;

public interface IOdsParser
{
    IReadOnlyList<T> ReadOds<T>(string filePath, string tableName, Func<DataRow, T> f);
}

public class OdsParser: IOdsParser
{
    public IReadOnlyList<T> ReadOds<T>(string filePath, string tableName, Func<DataRow, T> f)
    {
        var odsReaderWriter = new ODSReaderWriter();
        var spreadsheetData = odsReaderWriter.ReadOdsFile(filePath);
        var records = new List<T>();

        var table = spreadsheetData.Tables[tableName];
        bool isFirstRow = true;
        foreach (var row in table.AsEnumerable())
        {
            if (isFirstRow)
            {
                isFirstRow = false;
                continue;
            }

            var z = f(row);
            records.Add(z);
        }

        return records;
    }
}