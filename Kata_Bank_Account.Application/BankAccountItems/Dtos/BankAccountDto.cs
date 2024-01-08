using Kata_Bank_Account.Domain.Entities;

namespace Kata_Bank_Account.Application.BankAccountItems.Dtos
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }

        public int Amount { get; set; }

        public BankAccountDto(Guid id, int amount)
        {
            Id = id;
            Amount = amount;
        }

        public static BankAccountDto FromBankAccountEntity(BankAccount account) =>
            new(account.Id, account.Amount);
    }
}
