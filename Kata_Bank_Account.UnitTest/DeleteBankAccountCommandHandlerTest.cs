using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Kata_Bank_Account.Application.BankAccountItems.Command.Delete;
using Kata_Bank_Account.Application.Common.Exceptions;
using MediatR;
using Xunit;

namespace Kata_Bank_Account.UnitTest
{
    public class DeleteBankAccountCommandHandlerTest
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly CreateBankAccountCommandHandler _handlerBankAccount;
        private readonly DeleteBankAccountCommandHandler _handlerDeleteBankAccount;

        public DeleteBankAccountCommandHandlerTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Kata_Bank_Account")
                .Options;
            _handlerBankAccount = new CreateBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
            _handlerDeleteBankAccount = new DeleteBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
        }

        [Fact]
        public async Task DeleteAccount_Valid()
        {
            var requestBankAccount = new CreateBankAccountCommnand();
            var resultBankAccount = await _handlerBankAccount.Handle(requestBankAccount, default);

            Assert.NotNull(resultBankAccount);

            var requestDeleteBankAccount = new DeleteBankAccountCommand
            {
                AccountId = resultBankAccount.Id
            };

            var resultDeleteBankAccount = await _handlerDeleteBankAccount.Handle(requestDeleteBankAccount, default);
            Assert.Equal(Unit.Value, resultDeleteBankAccount);

        }

        [Fact]
        public async Task DeleteAccount_InvalidId_Invalid()
        {
            var requestDeleteBankAccount = new DeleteBankAccountCommand
            {
                AccountId = new Guid()
            };

            _ = Assert.ThrowsAsync<ValidationException>(() =>
                _handlerDeleteBankAccount.Handle(requestDeleteBankAccount, default));
        }
    }
}
