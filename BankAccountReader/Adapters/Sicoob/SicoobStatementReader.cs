using System.Globalization;
using System.Text.RegularExpressions;
using BankAccountReader.Domain;

namespace BankAccountReader.Adapters.Sicoob;

public class SicoobStatementReader : IStatementReaderAdapter
{
    private static CultureInfo _culture = new CultureInfo("pt-BR");
    private static string _valuePattern = @"(\d{2}/\d{2})\s+(.+?)\s+(-?\d{1,3}(?:\.\d{3})*,\d{2}[CD])";

    public CultureInfo Culture => _culture;

    public string[] ReadLines(string text)
    {
        string[] result = text.Split("\n");
        var currentLine = 1;
        foreach(var line in result)
        {
            if(line.Contains("SALDO BLOQ.ANTERIOR"))
            {
                return result.Skip(currentLine).ToArray();
            }
            currentLine++;
        }
        return result;
    }

    TransactionRecord[] IStatementReaderAdapter.ParseLine(string text)
    {
        var match = Regex.Match(text, _valuePattern);

        if(match.Success)
        {
            var amount = match.Groups[3].Value;
            var desc = match.Groups[2].Value;

            if(desc.Trim().Equals("SALDO DO DIA", StringComparison.InvariantCultureIgnoreCase))
            {
                return [];
            }
            
            amount = amount.EndsWith('D') ? $"-{amount.Replace("D", "")}" : amount.Replace("C", "");
            
            var item = new TransactionRecord
            {
                TransactionDate = DateOnly.Parse(match.Groups[1].Value, _culture),
                Description = desc,
                Amount = double.Parse(amount, NumberStyles.Currency, _culture)
            };

            return [item];
        }

        return [];
    }
}