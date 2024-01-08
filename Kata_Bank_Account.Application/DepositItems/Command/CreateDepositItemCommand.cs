using System.ComponentModel.DataAnnotations;
using Kata_Bank_Account.Application.Common.Enums;
using Kata_Bank_Account.Application.Interfaces;
using Kata_Bank_Account.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = Kata_Bank_Account.Application.Common.Exceptions.ValidationException;

namespace Kata_Bank_Account.Application.DepositItems.Command
{
    public class CreateDepositItemCommand : IRequest<Unit>
    {
       [Required] public Guid AccountId { get; set; }
       [Required] public int Amount { get; set; }
    }

    public class CreateDepositItemCommandHandler : IRequestHandler<CreateDepositItemCommand, Unit>
    {
        private readonly IAppDbContext _dbContext;

        public CreateDepositItemCommandHandler(IAppDbContext dbContext)
        {
                _dbContext = dbContext;
        }
        public async Task<Unit> Handle(CreateDepositItemCommand request, CancellationToken cancellationToken)
        {
            

            var account = await _dbContext.BankAccounts!.FirstOrDefaultAsync(n => n.Id == request.AccountId, cancellationToken: cancellationToken);


            if (account == null)
                throw new Common.Exceptions.ValidationException($"Account ID {request.AccountId} does not exist !");

            if (request.Amount <= 0)
                throw new ValidationException("The amount must be grater than 0 !");

            account.Amount += request.Amount;
            _dbContext.BankAccounts!.Update(account);


            //Add transaction to database
            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Amount = request.Amount,
                Type = TransactionType.Deposit.ToString(),
                TransactionDate = DateTime.Now
            };
            _dbContext.Transactions!.Add(transaction);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
