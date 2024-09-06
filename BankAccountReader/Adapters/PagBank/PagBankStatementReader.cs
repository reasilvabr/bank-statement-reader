using System.Globalization;
using System.Text.RegularExpressions;
using BankAccountReader.Domain;

namespace BankAccountReader.Adapters.PagBank;

public class PagBankStatementReader : IStatementReaderAdapter
{
    private static string _valuePattern = @"(\d{2}/\d{2}/\d{4})\s*(.*?)\s*(-?R\$\s*[0-9.,]+)";
    private static readonly CultureInfo _culture = CultureInfo.GetCultureInfo("pt-BR");

    public CultureInfo Culture => _culture;

    public TransactionRecord[] ParseLine(string text)
    {
        var match = Regex.Match(text, _valuePattern);

        if (match.Success)
        {
            var item = new TransactionRecord
            {
                Description = match.Groups[2].Value
            };

            if (item.Description.Equals("Saldo do dia", StringComparison.OrdinalIgnoreCase))
            {
                return [];
            }

            item.TransactionDate = DateOnly.Parse(match.Groups[1].Value, _culture);
            item.Amount = double.Parse(match.Groups[3].Value, NumberStyles.Currency, _culture);

            return [item];
        }

        return [];
    }

    public string[] ReadLines(string text)
    {
        return text.Split('\n');
    }
}