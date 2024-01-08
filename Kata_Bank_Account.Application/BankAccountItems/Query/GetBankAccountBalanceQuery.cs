using System.ComponentModel.DataAnnotations;
using Kata_Bank_Account.Application.BankAccountItems.Dtos;
using Kata_Bank_Account.Application.Interfaces;
using MediatR;
using ValidationException = Kata_Bank_Account.Application.Common.Exceptions.ValidationException;

namespace Kata_Bank_Account.Application.BankAccountItems.Query
{
    public class GetBankAccountBalanceQuery : IRequest<BankAccountDto>
    {
        [Required]
        public Guid BankAccountId { get; set; }
    }

    public class GetBankAccountBalanceQueryHandler : IRequestHandler<GetBankAccountBalanceQuery, BankAccountDto>, IRequest<BankAccountDto>
    {
        private readonly IAppDbContext _dbContext;

        public GetBankAccountBalanceQueryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BankAccountDto> Handle(GetBankAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.BankAccounts!.FindAsync(request.BankAccountId,cancellationToken);

            if (account == null)
                throw new ValidationException($"The account id {request.BankAccountId} does not exist !");

            BankAccountDto dto = BankAccountDto.FromBankAccountEntity(account);
            return dto;
        }
    }
}
