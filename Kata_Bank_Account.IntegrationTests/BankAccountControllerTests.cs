using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Kata_Bank_Account.Application.BankAccountItems.Command.Create;
using Kata_Bank_Account.Application.BankAccountItems.Command.Delete;
using Kata_Bank_Account.Application.BankAccountItems.Dtos;
using Kata_Bank_Account.Application.BankAccountItems.Query;
using Kata_Bank_Account.Application.Common.Exceptions;
using Kata_Bank_Account.Application.DepositItems.Command;
using Kata_Bank_Account.Application.WithdrawalItems.Command.Create;
using Kata_Bank_Account.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kata_Bank_Account.IntegrationTests
{
    public class BankAccountControllerTests
    {
        private readonly HttpClient _httpClient;
        public BankAccountControllerTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateClient();
        }

        #region BankAccountRegion

        [Fact]
        public async Task CreateAccountTest_Valid()
        {
            CreateBankAccountCommnand command = new CreateBankAccountCommnand();

            var response = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", command);

            response.EnsureSuccessStatusCode();

            var accountResponse = await response.Content.ReadFromJsonAsync<BankAccountDto>();
            accountResponse.Id.Should().NotBeEmpty();
            accountResponse.Amount.Should().BeGreaterOrEqualTo(0);



            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task GetAccountBalanceValid()
        {
            CreateBankAccountCommnand command = new CreateBankAccountCommnand();

            var response = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", command);

            response.EnsureSuccessStatusCode();

            var accountResponse = await response.Content.ReadFromJsonAsync<BankAccountDto>();

           

            var balanceResponse =
                await _httpClient.GetFromJsonAsync<BankAccountDto>($"bank-account/bankAccountBalance/{accountResponse!.Id}");

            balanceResponse.Should().NotBeNull();
            balanceResponse.Id.Should().NotBeEmpty();
            balanceResponse.Amount.Should().BeGreaterOrEqualTo(0);

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task GetAccountBalance__InvalidId_ThrowValidationException()
        {

           await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var balanceResponse =
                    await _httpClient.GetFromJsonAsync<BankAccountDto>($"bank-account/bankAccountBalance/{new Guid()}");
            });
        }


        [Fact]
        public async Task GetAccountTransactionHistory_Valid()
        {
            CreateBankAccountCommnand command = new CreateBankAccountCommnand();

            var response = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", command);

            response.EnsureSuccessStatusCode();

            var accountResponse = await response.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateDepositItemCommand commandDeposit = new CreateDepositItemCommand
            {
                AccountId = accountResponse!.Id,
                Amount = 1000
            };

            var responseDeposit =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/deposit", commandDeposit);

            responseDeposit.EnsureSuccessStatusCode();


            var responseTransactionHistory = await _httpClient.GetAsync($"bank-account/transaction/transactionHistory/{accountResponse.Id}");

            var transactionHistoryResponseBody = await responseTransactionHistory.Content.ReadFromJsonAsync<List<TransactionDto>>();

            responseTransactionHistory.IsSuccessStatusCode.Should().BeTrue();
            transactionHistoryResponseBody.Should().NotBeNull();
            
            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task GetAccountTransactionHistory_InvalidId_ThrowValidationException()
        {

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var transactionHistoryResponse =
                    await _httpClient.GetFromJsonAsync<Transaction>($"bank-account/transaction/transactionHistory/{new Guid()}");
            });
        }

        [Fact]
        public async Task DeleteAccount_Valid()
        {
            CreateBankAccountCommnand command = new CreateBankAccountCommnand();

            var response = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", command);

            response.EnsureSuccessStatusCode();

            var accountResponse = await response.Content.ReadFromJsonAsync<BankAccountDto>();

            var responseDelete = await _httpClient.DeleteAsync($"bank-account/delete/{accountResponse.Id}");

            responseDelete.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAccount_IdNotFound_Invalid()
        {
            var responseDelete = await _httpClient.DeleteAsync($"bank-account/delete/{new Guid()}");
          
            responseDelete.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }

        #endregion


        #region Deposit tests
        [Fact]
        public async Task MakeDepositToBankAccount_Valid()
        {
            CreateBankAccountCommnand commandCreateBankAccount = new CreateBankAccountCommnand();

            var responseBankAccount = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", commandCreateBankAccount);
            var accountResponse = await responseBankAccount.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateDepositItemCommand commandDeposit = new CreateDepositItemCommand
            {
                AccountId = accountResponse.Id,
                Amount = 1000
            };

            var responseDeposit =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/deposit", commandDeposit);

            responseDeposit.Should().Be(responseDeposit.EnsureSuccessStatusCode());

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task MakeDepositToBankAccount_InvalidId_ThrowValidationException()
        {

            CreateDepositItemCommand commandDeposit = new CreateDepositItemCommand
            {
                AccountId = new Guid(),
                Amount = 1000
            };

            var responseDeposit =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/deposit", commandDeposit);

            responseDeposit.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakeDepositToBankAccount_InvalidAmount_ThrowValidationException()
        {

            CreateBankAccountCommnand commandCreateBankAccount = new CreateBankAccountCommnand();

            var responseBankAccount = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", commandCreateBankAccount);
            var accountResponse = await responseBankAccount.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateDepositItemCommand commandDeposit = new CreateDepositItemCommand
            {
                AccountId = accountResponse.Id,
                Amount = -100
            };

            var responseDeposit =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/deposit", commandDeposit);

            responseDeposit.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }


        #endregion

        #region Withdrawal tests

        [Fact]
        public async Task MakeWithdrawalToBankAccount()
        {
            CreateBankAccountCommnand commandCreateBankAccount = new CreateBankAccountCommnand();

            var responseBankAccount = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", commandCreateBankAccount);
            var accountResponse = await responseBankAccount.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateDepositItemCommand commandDeposit = new CreateDepositItemCommand
            {
                AccountId = accountResponse.Id,
                Amount = 1000
            };

            var responseDeposit =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/deposit", commandDeposit);

            CreateWithdrawalItemsCommand commandWithdrawal = new CreateWithdrawalItemsCommand
            {
                AccountId = accountResponse.Id,
                Amount = 500
            };

            var responseWithdrawal =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/withdrawal", commandWithdrawal);

            responseDeposit.IsSuccessStatusCode.Should().BeTrue();
            

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task MakeWIthdrawalToBankAccount_InvalidId_ThrowValidationException()
        {

            CreateWithdrawalItemsCommand commandWithdrawal = new CreateWithdrawalItemsCommand
            {
                AccountId = new Guid(),
                Amount = 1000
            };

            var responseWithdrawal =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/withdrawal", commandWithdrawal);

            responseWithdrawal.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakeWIthdrawalToBankAccount_InvalidAmount_ThrowValidationException()
        {
            CreateBankAccountCommnand commandCreateBankAccount = new CreateBankAccountCommnand();

            var responseBankAccount = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", commandCreateBankAccount);
            var accountResponse = await responseBankAccount.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateWithdrawalItemsCommand commandWithdrawal = new CreateWithdrawalItemsCommand
            {
                AccountId = accountResponse.Id,
                Amount = 0
            };

            var responseWithdrawal =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/withdrawal", commandWithdrawal);

            responseWithdrawal.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        [Fact]
        public async Task MakeWIthdrawalToBankAccount_InvalidBalance_ThrowValidationException()
        {
            CreateBankAccountCommnand commandCreateBankAccount = new CreateBankAccountCommnand();

            var responseBankAccount = await _httpClient.PostAsJsonAsync("/bank-account/createBankAccount", commandCreateBankAccount);
            var accountResponse = await responseBankAccount.Content.ReadFromJsonAsync<BankAccountDto>();

            CreateWithdrawalItemsCommand commandWithdrawal = new CreateWithdrawalItemsCommand
            {
                AccountId = accountResponse.Id,
                Amount = 100
            };

            var responseWithdrawal =
                await _httpClient.PostAsJsonAsync("/bank-account/transaction/withdrawal", commandWithdrawal);

            responseWithdrawal.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            _ = await _httpClient.DeleteAsync($"/bank-account/delete/{accountResponse.Id}");
        }

        #endregion

    }
}