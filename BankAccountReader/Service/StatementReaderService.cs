using System.Text;
using BankAccountReader.Adapters;
using BankAccountReader.Adapters.PagBank;
using BankAccountReader.Adapters.Sicoob;
using BankAccountReader.Domain;

namespace BankAccountReader.Service;

public class StatementReaderService : IStatementReaderService
{
    private IStatementReaderAdapter? _reader;

    public IEnumerable<TransactionRecord> ReadStatement(string text, BankCode bank)
    {
        _reader = BuildReader(bank);
  
        var lines = _reader.ReadLines(text);
        var result = new List<TransactionRecord>();
  
        foreach (var line in lines)
        {
            try
            {
                result.AddRange(_reader.ParseLine(line));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return result;
    }

    public string ConvertToCsv(IEnumerable<TransactionRecord> records)
    {
        if(_reader == null)
        {
            throw new ArgumentNullException("Reader not set");
        }

        var result = new StringBuilder();
        var ci = _reader.Culture;
        result.AppendLine("Date\tDescription\tValue");
        foreach (var record in records)
        {
            result.AppendLine($"{record.TransactionDate.ToString(ci)}\t{record.Description}\t{record.Amount.ToString(ci)}");
        }
        return result.ToString();
    }

    private IStatementReaderAdapter BuildReader(BankCode bank)
    {
        switch (bank)
        {
            case BankCode.PagBank:
                return new PagBankStatementReader();
            case BankCode.Siscoob:
                return new SicoobStatementReader();
            default:
                throw new NotImplementedException();
        }
    }
}