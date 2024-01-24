using System.ComponentModel.DataAnnotations;

namespace Kata_Bank_Account.Domain.Entities
{
    public class BankAccount
    {
        [Key]
        public Guid Id { get; set; }
        public int Amount { get; set; }
    }
}
