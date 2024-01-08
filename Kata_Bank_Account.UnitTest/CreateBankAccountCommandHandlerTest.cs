using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Kata_Bank_Account.UnitTest
{
    public class CreateBankAccountCommandHandlerTest
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly CreateBankAccountCommandHandler _handler;

        public CreateBankAccountCommandHandlerTest()
        {
            // Use in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Kata_Bank_Account")
                .Options;

            _handler = new CreateBankAccountCommandHandler(new AppDbContext(_dbContextOptions));
        }

        [Fact]
        public async Task CreateBankAccountValid()
        {

            var request = new CreateBankAccountCommnand();


            var result = await _handler.Handle(request, default);


            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);


            using (var dbContext = new AppDbContext(_dbContextOptions))
            {
                var createdBankAccount = await dbContext.BankAccounts!.FindAsync(result.Id);
                Assert.NotNull(createdBankAccount);
            }
        }
    }

}