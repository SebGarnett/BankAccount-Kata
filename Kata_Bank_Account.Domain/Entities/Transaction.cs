using System.ComponentModel.DataAnnotations;

namespace Kata_Bank_Account.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

   
}
