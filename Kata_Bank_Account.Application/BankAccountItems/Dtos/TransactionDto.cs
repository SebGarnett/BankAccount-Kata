using Transaction = Kata_Bank_Account.Domain.Entities.Transaction;

namespace Kata_Bank_Account.Application.BankAccountItems.Dtos
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public Guid AccountId { get; set; }
        public string TransactionType { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public TransactionDto(int transactionId, Guid accountId, string transactionType, int amount, DateTime transactionDate)
        {
            TransactionId = transactionId;
            AccountId = accountId;
            TransactionType = transactionType;
            Amount = amount;
            TransactionDate = transactionDate;
        }

        public static TransactionDto FromTransactionEntity(Transaction transaction) =>
            new(transaction.Id, transaction.AccountId, transaction.Type, transaction.Amount,
                transaction.TransactionDate);
    }
}
