using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.DepositItems.Command;
using Kata_Bank_Account.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kata_Bank_Account.UnitTest;

public class CreateDepositCommandHandlerTest
{

    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly CreateBankAccountCommandHandler _handlerBankAccount;
    private readonly CreateDepositItemCommandHandler _handler;

    public CreateDepositCommandHandlerTest()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Kata_Bank_Account")
            .Options;
        _handlerBankAccount = new CreateBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
        _handler = new CreateDepositItemCommandHandler(new AppDbContext(_dbContextOptions));
    }

    [Fact]
    public async Task CreateDepositValid()
    {
        var requestBankAccount = new CreateBankAccountCommnand();
        var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

        var request = new CreateDepositItemCommand
        {
            AccountId = resultBankAccount.Id,
            Amount = 1000
        };

        var resultDeposit = await _handler.Handle(request, default);

       Assert.Equal(Unit.Value, resultDeposit);

    }

    [Fact]
    public async Task NotExistingAccount_ThrowsException()
    {
        var request = new CreateDepositItemCommand
        {
            AccountId = new Guid(),
            Amount = 1000
        };

       await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, default));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task NotAcceptedAmount_ThrowsException(int amount)
    {
        var request = new CreateDepositItemCommand
        {
            AccountId = new Guid(),
            Amount = amount
        };

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, default));
    }
}