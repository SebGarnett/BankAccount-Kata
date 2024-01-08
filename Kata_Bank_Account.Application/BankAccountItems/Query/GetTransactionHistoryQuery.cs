using System.ComponentModel.DataAnnotations;
using Kata_Bank_Account.Application.BankAccountItems.Dtos;
using Kata_Bank_Account.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kata_Bank_Account.Application.BankAccountItems.Query
{
    public class GetTransactionHistoryQuery : IRequest<List<TransactionDto>>
    {
        [Required]
        public Guid AccountId { get; set; }
    }

    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery,List<TransactionDto>>
    {
        private readonly IAppDbContext _dbContext;

        public GetTransactionHistoryQueryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            List<TransactionDto> listDtos = new List<TransactionDto>();

            var transactions = await _dbContext.Transactions!.Where(n => n.AccountId == request.AccountId).ToListAsync(cancellationToken);
            if (transactions.Count == 0)
                throw new InvalidOperationException($"No transactions found for this account id : {request.AccountId}");


            foreach (var tran in transactions)
            {
                listDtos.Add(TransactionDto.FromTransactionEntity(tran));
            }

            return listDtos;
        }
    }
}
