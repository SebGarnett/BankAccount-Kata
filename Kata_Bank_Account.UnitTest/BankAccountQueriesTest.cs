using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Application.BankAccountItems.Query;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.DepositItems.Command;
using Kata_Bank_Account.Application.WithdrawalItems.Command.Create;
using Kata_Bank_Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kata_Bank_Account.UnitTest;

public class BankAccountQueriesTest
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly CreateBankAccountCommandHandler _handlerBankAccount;
    private readonly CreateDepositItemCommandHandler _handlerDeposit;
    private readonly CreateWithdrawalItemsCommandHandler _handlerWithdraw;
    private readonly GetBankAccountBalanceQueryHandler _handlerBalance;
    private readonly GetTransactionHistoryQueryHandler _handlerTransactionHistory;


    public BankAccountQueriesTest()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "Kata_Bank_Account")
            .Options;
        _handlerBankAccount = new CreateBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
        _handlerDeposit = new CreateDepositItemCommandHandler(new AppDbContext(_dbContextOptions));
        _handlerWithdraw = new CreateWithdrawalItemsCommandHandler(new AppDbContext(_dbContextOptions));
        _handlerBalance = new GetBankAccountBalanceQueryHandler(new AppDbContext(_dbContextOptions));
        _handlerTransactionHistory = new GetTransactionHistoryQueryHandler(new AppDbContext(_dbContextOptions));
    }

    [Fact]
    public async Task GetAccountBalanceValid()
    {
        var requestBankAccount = new CreateBankAccountCommnand();
        var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

        var requestDeposit = new CreateDepositItemCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 1000
        };

        var resultDeposit = await _handlerDeposit.Handle(requestDeposit, default);

        int balanceExpected = resultBankAccount.Amount + requestDeposit.Amount;


        var requestBalance = new GetBankAccountBalanceQuery
        {
            BankAccountId = resultBankAccount.Id
        };

       var resultBalance = await _handlerBalance.Handle(requestBalance,default);
       Assert.NotNull(resultBalance);

       Assert.Equal(resultBalance.Amount, balanceExpected);
    }

    [Fact]
    public async Task BalanceAccountNotFound_ThrowException()
    {
        var requestBalance = new GetBankAccountBalanceQuery
        {
            BankAccountId = new Guid()
        };

       await Assert.ThrowsAsync<ValidationException>(() => _handlerBalance.Handle(requestBalance, default));
    }

    [Fact]
    public async Task GetTransactionHistoryValid()
    {
        var requestBankAccount = new CreateBankAccountCommnand();
        var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

        var requestDeposit = new CreateDepositItemCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 1000
        };

        _ = await _handlerDeposit.Handle(requestDeposit, default);

        var requestWithdraw = new CreateWithdrawalItemsCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 500
        };

        _ = await _handlerWithdraw.Handle(requestWithdraw, default);

        var requestTransactionHistory = new GetTransactionHistoryQuery
        {
            AccountId = resultBankAccount.Id
        };

        var resultTransactionHistory = _handlerTransactionHistory.Handle(requestTransactionHistory, default);

        Assert.NotNull(resultTransactionHistory);
    }

    [Fact]
    public async Task TransactionHistoryAccountNotFound_ThrowException()
    {
        var requestTransactionHistory = new GetTransactionHistoryQuery
        {
            AccountId = new Guid()
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handlerTransactionHistory.Handle(requestTransactionHistory, default));
    }
}