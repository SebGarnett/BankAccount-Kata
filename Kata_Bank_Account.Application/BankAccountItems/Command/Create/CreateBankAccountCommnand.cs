using Kata_Bank_Account.Application.BankAccountItems.Dtos;
using Kata_Bank_Account.Application.Interfaces;
using Kata_Bank_Account.Domain.Entities;
using MediatR;

namespace Kata_Bank_Account.Application.BankAccountItems.Command.Create
{
    public class CreateBankAccountCommnand : IRequest<BankAccountDto>
    {
    }

    public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommnand, BankAccountDto>
    {
        private readonly IAppDbContext _dbContext;

        public CreateBankAccountCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BankAccountDto> Handle(CreateBankAccountCommnand request, CancellationToken cancellationToken)
        {
            BankAccount bankAccount = new BankAccount
            {
                Id = new Guid(),
                Amount = 0
            };
            await _dbContext.BankAccounts!.AddAsync(bankAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return BankAccountDto.FromBankAccountEntity(bankAccount); ;
        }
    }
}
