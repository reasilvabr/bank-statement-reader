namespace BankAccountReader.Domain;

public class TransactionRecord
{
    public DateOnly TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string? Notes { get; set; }
}