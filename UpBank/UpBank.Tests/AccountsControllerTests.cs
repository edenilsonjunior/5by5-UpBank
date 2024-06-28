using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.DTO;
using UpBank.AccountAPI.Controllers;

namespace UpBank.Tests;

public class AccountsControllerTests : IDisposable
{
    private MockData _mockData;
    private readonly AccountsController _accountsController;

    public AccountsControllerTests()
    {
        _mockData = new();
        _accountsController = new AccountsController();
    }


    // Testing Get endpoints

    [Fact]
    public async Task GetAllAccounts_ReturnsOkResult()
    {
        var result = await _accountsController.GetAllAccounts();
        var accounts = (result.Result as OkObjectResult).Value as List<Account>;

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((result.Result as OkObjectResult).Value is List<Account>);
        Assert.True(accounts.Count > 0);

        Assert.Contains(accounts, a => a.Number == "1001");
        Assert.Contains(accounts, a => a.Number == "1002");
        Assert.Contains(accounts, a => a.Number == "1003");
    }


    [Fact]
    public async Task GetAccount_WithValidNumber_ReturnsOkResult()
    {
        var accountNumber = "1001";
        var result = await _accountsController.GetAccount(accountNumber);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull((result.Result as OkObjectResult).Value);
    }


    [Fact]
    public async Task GetAccount_WithInvalidNumber_ReturnsNotFoundResult()
    {
        var accountNumber = "312312323";
        var result = await _accountsController.GetAccount(accountNumber);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    [Fact]
    public async Task GetTransactionByType_ReturnsOkResult()
    {

        var depositTask = _accountsController.GetTransactionByType("Deposit");
        var withdrawTask = _accountsController.GetTransactionByType("Withdraw");
        var lendingTask = _accountsController.GetTransactionByType("Lending");
        var paymentTask = _accountsController.GetTransactionByType("Payment");
        var transferTask = _accountsController.GetTransactionByType("Transfer");

        await Task.WhenAll(depositTask, withdrawTask, lendingTask, paymentTask, transferTask);

        // Deposit
        Assert.IsType<OkObjectResult>(depositTask.Result.Result);
        Assert.True((depositTask.Result.Result as OkObjectResult)?.Value is List<BankTransaction>);
        Assert.True(((depositTask.Result.Result as OkObjectResult)?.Value as List<BankTransaction>).Count == 4);

        // Withdraw
        Assert.IsType<OkObjectResult>(withdrawTask.Result.Result);
        Assert.True((withdrawTask.Result.Result as OkObjectResult)?.Value is List<BankTransaction>);
        Assert.True(((withdrawTask.Result.Result as OkObjectResult)?.Value as List<BankTransaction>)?.Count == 3);

        // Lending
        Assert.IsType<OkObjectResult>(lendingTask.Result.Result);
        Assert.True((lendingTask.Result.Result as OkObjectResult)?.Value is List<BankTransaction>);
        Assert.True(((lendingTask.Result.Result as OkObjectResult)?.Value as List<BankTransaction>)?.Count == 2);

        // Payment
        Assert.IsType<OkObjectResult>(paymentTask.Result.Result);
        Assert.True((paymentTask.Result.Result as OkObjectResult)?.Value is List<BankTransaction>);
        Assert.True(((paymentTask.Result.Result as OkObjectResult)?.Value as List<BankTransaction>)?.Count == 3);

        // Transfer
        Assert.IsType<OkObjectResult>(transferTask.Result.Result);
        Assert.True((transferTask.Result.Result as OkObjectResult)?.Value is List<BankTransaction>);
        Assert.True(((transferTask.Result.Result as OkObjectResult)?.Value as List<BankTransaction>)?.Count == 4);
    }


    [Fact]
    public async Task GetTransactionByType_WithInvalidType_ReturnsNotFoundResult()
    {

        var transactionType = "withdrawzinho";
        var result = await _accountsController.GetTransactionByType(transactionType);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    [Fact]
    public async Task GetBankStatement_WithValidAccountNumber_ReturnsOkResult()
    {
        var accountNumber = "1001";
        var result = await _accountsController.GetBankStatement(accountNumber);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);

        var list = (result.Result as OkObjectResult).Value as List<BankTransaction>;

        Assert.True(list is List<BankTransaction>);
        Assert.True(list.Count == 14);

        // Verifing the number of transactions by type
        Assert.True(list.Where(list => list.Type == ETransactionType.Withdraw).Count() == 3);
        Assert.True(list.Where(list => list.Type == ETransactionType.Deposit).Count() == 4);
        Assert.True(list.Where(list => list.Type == ETransactionType.Lending).Count() == 0);
        Assert.True(list.Where(list => list.Type == ETransactionType.Payment).Count() == 3);
        Assert.True(list.Where(list => list.Type == ETransactionType.Transfer).Count() == 4);
    }


    [Fact]
    public async Task GetBalance_WithValidAccountNumber_ReturnsOkResult()
    {
        // Arrange
        var accountNumber = "1001";

        // Act
        var result = await _accountsController.GetBalance(accountNumber);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);

        var balance = (result.Result as OkObjectResult).Value as BalanceDTO;

        Assert.True(balance is not null);

        Assert.Equal("Ana Maria Silva", balance.OwnerName);
        Assert.Equal(10000, balance.Balance);
        Assert.Equal(6000, balance.Overdraft);
        Assert.Equal(3500, balance.CreditCardLimit);
    }


    [Fact]
    public async Task GetBalance_WithInvalidAccountNumber_ReturnsNotFoundResult()
    {
        var accountNumber = "123456";
        var result = await _accountsController.GetBalance(accountNumber);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    // Testing Post endpoints

    [Fact]
    public async Task CreateAccount_WithNotRegistredCpf_ReturnsBadRequestResult()
    {
        var accountDTO = new AccountDTO()
        {
            AccountNumber = "1004",
            AgencyNumber = "4415",
            AccountProfile = "Normal",
            ClientCPF = new List<string> { "22233344455" },   // CPF not registered
            Overdraft = 500,
            CreditCardLimit = 5000.00,
            CreditCardHolder = "John Doe"
        };

        var result = await _accountsController.CreateAccount(accountDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task CreateAccount_WithUnderAgeClient_ReturnsBadRequestResult()
    {
        var accountDTO = new AccountDTO()
        {
            AccountNumber = "1004",
            AgencyNumber = "4415",
            AccountProfile = "Normal",
            ClientCPF = new List<string> { "39284712069" },   // Under age
            Overdraft = 500,
            CreditCardLimit = 5000.00,
            CreditCardHolder = "John Doe"
        };

        var result = await _accountsController.CreateAccount(accountDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task CreateAccount_WithRestrictedClient_ReturnsBadRequestResult()
    {
        var accountDTO = new AccountDTO()
        {
            AccountNumber = "1004",
            AgencyNumber = "4415",
            AccountProfile = "Normal",
            ClientCPF = new List<string> { "71928364052" },   // Restricted cpf
            Overdraft = 500,
            CreditCardLimit = 5000.00,
            CreditCardHolder = "John Doe"
        };

        var result = await _accountsController.CreateAccount(accountDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task CreateAccount_WithAnExistingAccountNumber_ReturnsBadRequestResult()
    {
        var accountDTO = new AccountDTO()
        {
            AccountNumber = "1001",     // Existing account number
            AgencyNumber = "4415",
            AccountProfile = "Normal",
            ClientCPF = new List<string> { "65317260086" },
            Overdraft = 500,
            CreditCardLimit = 5000.00,
            CreditCardHolder = "John Doe"
        };

        var result = await _accountsController.CreateAccount(accountDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task MakeTransaction_WithValidTransactionDTO_ReturnsOkResult()
    {
        TransactionDTO transaction = new()
        {
            AccountNumber = "1001",
            TransactionDt = DateTime.Now,
            TransactionType = "Lending",
            ReceiverAccount = null,
            TransactionValue = 1000
        };

        var result = await _accountsController.MakeTransaction(transaction);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    public void Dispose()
    {
        _mockData.Dispose();
    }
}
