using EnergyChecker.DataAccess.Entities;
using EnergyChecker.Utils;
using EnergyChecker.Utils.Parsers;

namespace EnergyChecker.DataAccess.Seed;

public class AccountSeeder
{
    public static void Seed(string filePath, IApplicationDbContext context)
    {
        var parserFactory = new ParserFactory();
        var parser = parserFactory.CreateOdsParser();
        var data = parser.ReadOds(filePath, "in", row => new AccountEntity
        {
            Id = long.Parse(row[0].ToString()),
            FirstName = row[1].ToString(),
            LastName = row[2].ToString()
        });

        context.Accounts.AddRange(data);
    }
}