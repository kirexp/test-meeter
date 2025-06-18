namespace EnergyChecker.Utils.Parsers;

public class ParserFactory
{
    public ICsvParser CreateCsvParser()
    {
        return new CsvParser();
    }

    public IOdsParser CreateOdsParser()
    {
        return new OdsParser();
    }
}