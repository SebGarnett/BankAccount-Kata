using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kata_Bank_Account.Application.BankAccountItems.Command.Delete
{
    public class DeleteBankAccountCommand : IRequest<Unit>
    {
        [Required] public Guid AccountId { get; set; }
    }

    public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, Unit>
    {
        private readonly IAppDbContext _dbContext;

        public DeleteBankAccountCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.BankAccounts!.FirstOrDefaultAsync(n => n.Id == request.AccountId, cancellationToken: cancellationToken);


            if (account == null)
                throw new Common.Exceptions.ValidationException($"Account ID {request.AccountId} does not exist !");


            _dbContext.Transactions?.RemoveRange(_dbContext.Transactions.Where(n => n.AccountId == request.AccountId));
            _dbContext.BankAccounts?.RemoveRange(_dbContext.BankAccounts.Where(n => n.Id == request.AccountId));
            if (await _dbContext.SaveChangesAsync(cancellationToken) > 0)
            {
                return Unit.Value;
            }

            throw new AccountNotDeletedException($"The account {request.AccountId} cannot be deleted ! Please contact administrator for further investigation ");
        }
    }
}
