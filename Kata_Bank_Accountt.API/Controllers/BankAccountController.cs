using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Application.BankAccountItems.Command.Delete;
using Kata_Bank_Account.Application.BankAccountItems.Dtos;
using Kata_Bank_Account.Application.BankAccountItems.Query;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.DepositItems.Command;
using Kata_Bank_Account.Application.WithdrawalItems.Command.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kata_Bank_Account.API.Controllers
{
    /// <summary>
    /// All the controllers for the bank account API
    /// </summary>
    public class BankAccountController : ApiBaseController
    {
        /// <summary>
        /// Creates a bank account (you don't need to pass a json body)
        /// </summary>
        /// <param name="command"></param>
        /// <response code="200">Account created successfully</response>
        [HttpPost("/bank-account/createBankAccount")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        public async Task<BankAccountDto> Create(CreateBankAccountCommnand command)
        {
            return await Mediator.Send(command);
        }
        /// <summary>
        /// Makes a deposit into your bank account
        /// </summary>
        /// <param name="command"></param>
        /// <response code="200">Deposit made successfully</response>
        /// <response code="400">Validation error. Could be : Account ID doesn't exist or The amount must be grater than 0</response>
        [HttpPost("/bank-account/transaction/deposit")]
        [ProducesResponseType(typeof(Unit), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), 400)]
        public async Task<ActionResult<Unit>> Create(CreateDepositItemCommand command)
        {
            return await Mediator.Send(command);
        }
        /// <summary>
        /// Make a withdraw from your bank account
        /// </summary>
        /// <param name="command"></param>
        /// <response code="200">Withdraw made successfully</response>
        /// <response code="400">Validation error. Could be : Account ID doesn't exist/The amount must be grater than 0/Not enough funds on your account</response>
        [HttpPost("/bank-account/transaction/withdrawal")]
        [ProducesResponseType(typeof(Unit), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), 400)]
        public async Task<ActionResult<Unit>> Create(CreateWithdrawalItemsCommand command)
        {
            return await Mediator.Send(command);
        }
        /// <summary>
        /// Get your account balance
        /// </summary>
        /// <param name="query"></param>
        /// <response code="200">Account balance retrieved successfully</response>
        /// <response code="400">Validation error. Account ID doesn't exist</response>
        [HttpGet("/bank-account/bankAccountBalance/{Id}")]
        [ProducesResponseType(typeof(BankAccountDto), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), 400)]
        public async Task<BankAccountDto> GetBankAccountBalance(Guid Id)
        {
            GetBankAccountBalanceQuery query = new GetBankAccountBalanceQuery
            {
                BankAccountId = Id
            };
            return await Mediator.Send(query);
        }
        /// <summary>
        /// Get all transactions made from your account
        /// </summary>
        /// <param name="query"></param>
        /// <response code="200">Account transaction history retrieved successfully</response>
        /// <response code="400">Validation error. Account ID doesn't exist</response>
        [HttpGet("/bank-account/transaction/transactionHistory/{Id}")]
        [ProducesResponseType(typeof(List<TransactionDto>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), 400)]
        public async Task<List<TransactionDto>> GetTransactionHistoryFromAccountId(
            Guid Id)
        {

            GetTransactionHistoryQuery query = new GetTransactionHistoryQuery
            {
                AccountId = Id
            };

            return await Mediator.Send(query);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        [HttpDelete("/bank-account/delete/{id}")]
        [ProducesResponseType(typeof(Unit), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 500)]
        public async Task<Unit> DeleteBankAccountFromAccountId(Guid Id)
        {
            DeleteBankAccountCommand command = new DeleteBankAccountCommand
            {
                AccountId = Id
            };

            return await Mediator.Send(command);

        }
    }
}
