using System.Globalization;
using BankAccountReader.Domain;

namespace BankAccountReader.Adapters;

public interface IStatementReaderAdapter {
    string[] ReadLines(string text);
    TransactionRecord[] ParseLine(string text);
    CultureInfo Culture {get;} 
}