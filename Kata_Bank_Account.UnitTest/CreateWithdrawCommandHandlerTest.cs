using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Application.DepositItems.Command;
using Kata_Bank_Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.WithdrawalItems.Command.Create;
using MediatR;
using Xunit;

namespace Kata_Bank_Account.UnitTest;

public class CreateWithdrawCommandHandlerTest
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly CreateBankAccountCommandHandler _handlerBankAccount;
    private readonly CreateDepositItemCommandHandler _handlerDeposit;
    private readonly CreateWithdrawalItemsCommandHandler _handlerWithdraw;

    public CreateWithdrawCommandHandlerTest()
    { 
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "Kata_Bank_Account")
            .Options;
        _handlerBankAccount = new CreateBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
        _handlerDeposit = new CreateDepositItemCommandHandler(new AppDbContext(_dbContextOptions));
        _handlerWithdraw = new CreateWithdrawalItemsCommandHandler(new AppDbContext(_dbContextOptions));
    }

    [Fact]
    public async Task CreateWithdrawValid()
    {
        var requestBankAccount = new CreateBankAccountCommnand();
        var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

        var requestDeposit = new CreateDepositItemCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 1000
        };

        var resultDeposit = await _handlerDeposit.Handle(requestDeposit, default);

        var requestWithdraw = new CreateWithdrawalItemsCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 500
        };

        var resultWithdraw = await _handlerWithdraw.Handle(requestWithdraw, default);

        Assert.Equal(Unit.Value, resultWithdraw);
    }

    [Fact]
    public async Task NotExistingAccount_ThrowsException()
    {
        var requestWithdraw = new CreateWithdrawalItemsCommand
        {
            AccountId = new Guid(),
            Amount = 500
        };

        await Assert.ThrowsAsync<ValidationException>(() => _handlerWithdraw.Handle(requestWithdraw, default));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task NotAcceptedAmount_ThrowsException(int amount)
    {
        var requestWithdraw = new CreateWithdrawalItemsCommand
        {
            AccountId = new Guid(),
            Amount = amount
        };

        await Assert.ThrowsAsync<ValidationException>(() => _handlerWithdraw.Handle(requestWithdraw, default));
    }

    [Theory]
    [InlineData(1000, 2000)]
    public async Task NotEnoughFunds_ThrowsException(int amountDeposit, int amountWithdraw)
    {
        var requestBankAccount = new CreateBankAccountCommnand();
        var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

        var requestDeposit = new CreateDepositItemCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = amountDeposit
        };

        var resultDeposit = await _handlerDeposit.Handle(requestDeposit, default);

        var requestWithdraw = new CreateWithdrawalItemsCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = amountWithdraw
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handlerWithdraw.Handle(requestWithdraw, default));
    }
}