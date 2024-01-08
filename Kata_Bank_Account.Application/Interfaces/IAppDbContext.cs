using Kata_Bank_Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kata_Bank_Account.Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<BankAccount>? BankAccounts { get; set; }
    public DbSet<Transaction>? Transactions { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}