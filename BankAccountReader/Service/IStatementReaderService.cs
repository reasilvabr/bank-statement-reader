using BankAccountReader.Domain;

namespace BankAccountReader.Service;

public interface IStatementReaderService
{
    IEnumerable<TransactionRecord> ReadStatement(string text, BankCode bank);
    string ConvertToCsv(IEnumerable<TransactionRecord> records);
}