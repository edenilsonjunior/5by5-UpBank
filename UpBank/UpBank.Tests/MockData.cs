using Dapper;
using Models.People;
using MongoDB.Driver;
using System.Data.SqlClient;

namespace UpBank.Tests;

public class MockData : IDisposable
{
    public static readonly string _accountConnStr = "Data Source=127.0.0.1; Initial Catalog=DBAccountUpBank; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";
    public static readonly string _agencyConnStr = "Data Source=127.0.0.1; Initial Catalog=UpBank.AgencyAPI; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";
    public static readonly string _employeeConnStr = "Data Source=127.0.0.1; Initial Catalog=DBEmployee; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";
    public static readonly string _clientConnStr = "Data Source=127.0.0.1; Initial Catalog=UpBankClientAPI; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";

    public MockData()
    {
        // Clear all tables
        Dispose();

        GenerateMongoAddresses();
        GenerateEmployeeMock();
        GenerateClientMock();
        GenerateAgencyMock();
        GenerateAccountMock();
        GenerateTransactionsMock();
    }


    private void GenerateMongoAddresses()
    {
        var client = new MongoClient("mongodb://root:Mongo%402024%23@localhost:27017");
        var database = client.GetDatabase("DbAddressUp");

        var collection = database.GetCollection<Address>("Address");


        Address a1 = new()
        {
            Id = "667c565b1573a059d574ce3d",
            Street = "Rua Petrópolis",
            Number = 10,
            Complement = "casa",
            ZipCode = "69915-062",
            City = "Rio Branco",
            State = "AC",
            District = "Loteamento Ipanema"
        };

        Address a2 = new()
        {
            Id = "667c56b81573a059d574ce3e",
            Street = "Alameda Anísio Paulo de Lucena",
            Number = 30,
            Complement = "apt",
            ZipCode = "69304-002",
            City = "Boa Vista",
            State = "RR",
            District = "Mecejana"
        };

        Address a3 = new()
        {
            Id = "667c5983793d71b722f6cf3a",
            Street = "Rua Sandra Gomes",
            Number = 123,
            Complement = "Apt 101",
            ZipCode = "88133-640",
            City = "Palhoça",
            State = "SC",
            District = "Brejarú"
        };

        Address a4 = new()
        {
            Id = "667c599f793d71b722f6cf3b",
            Street = "Rua São Vicente",
            Number = 123,
            Complement = "Apt 101",
            ZipCode = "69900-439",
            City = "Rio Branco",
            State = "AC",
            District = "Bosque"
        };

        Address a5 = new()
        {
            Id = "667c5a44f543f96f343fd5ef",
            Street = "Rua São Vicente",
            Number = 123,
            Complement = "Apt 101",
            ZipCode = "69900-439",
            City = "Rio Branco",
            State = "AC",
            District = "Bosque"
        };
        try
        {
            collection.InsertMany(new List<Address> { a1, a2, a3, a4, a5 });

        }
        catch (Exception)
        {
        }
    }


    private void GenerateEmployeeMock()
    {
        string queryEmployee = @"
        INSERT INTO [DBEmployee].[dbo].[Table_Employee] 
        (Name, CPF, BirthDt, Sex, IdAddress, Salary, Phone, Email, Manager, Registry)
        VALUES 
        ('Funcionario', '31143524055', '2010-05-15T00:00:00', 'M', '667c565b1573a059d574ce3d', 4000.50, '(11) 98765-4321', 'funcionario@example.com', 1, 1)";

        Execute(_employeeConnStr, queryEmployee);
    }


    private void GenerateAgencyMock()
    {
        string queryAgency = @"
        INSERT INTO [UpBank.AgencyAPI].[dbo].[Agency] 
        (Number, AddressId, CNPJ, Restriction)
        VALUES ('4415', '667c56b81573a059d574ce3e', '12.345.678/0001-99', 0)";

        string queryEmployeeAgency = @"
        INSERT INTO [UpBank.AgencyAPI].[dbo].[Employee]
        (CPF, Name, BirthDt, Sex, AddressId, Salary, Phone, Email, Manager, Registry, AgencyNumber)
        VALUES 
        ('31143524055', 'Funcionario', '2010-05-15T00:00:00', 'M', '667c565b1573a059d574ce3d', 4000.50, '(11) 98765-4321', 'funcionario@example.com', 1, 1, '4415')";

        Execute(_agencyConnStr, queryAgency);
        Execute(_agencyConnStr, queryEmployeeAgency);
    }


    private void GenerateClientMock()
    {
        string queryClient = @"
        INSERT INTO [UpBankClientAPI].[dbo].[Clients] 
        (CPF, Name, BirthDt, Sex, AddressId, Salary, Phone, Email, Restriction)
        VALUES 
        ('64783003076', 'Ana Maria Silva', '1990-06-12', 'F', '667c5983793d71b722f6cf3a', 2500.00, '(21) 91234-5678', 'ana.silva@example.com', 0),
        ('65317260086', 'Carlos Eduardo Souza', '1982-11-04', 'M', '667c599f793d71b722f6cf3b', 3200.00, '(31) 92345-6789', 'carlos.souza@example.com', 0),
        ('12541068042', 'Beatriz Oliveira', '1975-03-20', 'F', '667c5a44f543f96f343fd5ef', 1500.00, '(41) 93456-7890', 'beatriz.oliveira@example.com', 0),
        ('71928364052', 'João Pedro Lima', '1988-09-15', 'M', '667c5983793d71b722f6cf3a', 2800.00, '(11) 94567-8910', 'joao.lima@example.com', 1),
        ('39284712069', 'Mariana Ferreira', '2010-12-01', 'F', '667c599f793d71b722f6cf3b', 3400.00, '(51) 95678-9102', 'mariana.ferreira@example.com', 0);";

        /* 
         * Ana maria silva -> already has an account
         * Carlos Eduardo Souza -> already has an account
         * Beatriz Oliveira -> already has an account

         * João Pedro Lima -> Does not have an account and has restriction
         * Mariana Ferreira -> Does not have an account and is a minor
        */

        Execute(_clientConnStr, queryClient);
    }


    private void GenerateAccountMock()
    {
        string queryCreditCard = @"
                INSERT INTO [DBAccountUpBank].[dbo].[CreditCard] 
                (CreditCardNumber, ExpirationDt, CreditCardLimit, Cvv, Holder, Flag, Active)
                VALUES 
                (5518593462108232, '2029-06-26', 3500, 973, 'Ana Maria Silva', 'American Express', 1),
                (5142473035102760, '2029-06-26', 10000, 945, 'Carlos Eduardo Souza', 'MasterCard', 1),
                (5478254356487202, '2029-06-26', 1300, 779, 'Beatriz Oliveira', 'MasterCard', 1)";

        string queryAccount = @"
                INSERT INTO Account
                (AccountNumber, AgencyNumber, SavingAccountNumber, Restriction, CreditCardNumber, Overdraft, AccountProfile, CreatedDt, Balance)
                VALUES 
                (1001, '4415', '1-37', 0, 5518593462108232, 6000, 'Normal', '2024-06-26', 10000),
                (1002, '4415', '2-32', 0, 5142473035102760, 30000, 'VIP', '2024-06-26', 5000),
                (1003, '4415', '3-63', 0, 5478254356487202, 2000, 'Academic', '2024-06-26', 0)";

        string queryClientAccount = @"
                INSERT INTO ClientAccount 
                (AccountNumber, ClientCPF)
                VALUES 
                (1001, '64783003076'),
                (1002, '65317260086'),
                (1003, '12541068042')";

        using var connection = new SqlConnection(_accountConnStr);
        connection.Open();
        connection.Execute(queryCreditCard);
        connection.Execute(queryAccount);
        connection.Execute(queryClientAccount);
    }


    private void GenerateTransactionsMock()
    {
        string insert = @"INSERT INTO AccountTransaction (AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue)
                            Values
                            ('1001', '2022-06-26', 'Deposit', NULL, 323),
                            ('1001', '2022-06-26', 'Deposit', NULL, 10),
                            ('1001', '2022-06-26', 'Deposit', NULL, 45),
                            ('1001', '2022-06-26', 'Deposit', NULL, 932),

                            ('1001', '2022-06-26', 'Withdraw', NULL, 500),
                            ('1001', '2022-06-26', 'Withdraw', NULL, 300),
                            ('1001', '2022-06-26', 'Withdraw', NULL, 145),

                            ('1002', '2022-06-26', 'Lending', NULL, 17000),
                            ('1002', '2022-06-26', 'Lending', NULL, 3000),

                            ('1001', '2022-06-26', 'Payment', '1002', 1000),
                            ('1001', '2022-06-26', 'Payment', '1002', 2000),
                            ('1001', '2022-06-26', 'Payment', '1002', 3000),

                            ('1001', '2022-06-26', 'Transfer', '1002', 1000),
                            ('1001', '2022-06-26', 'Transfer', '1002', 2000),
                            ('1001', '2022-06-26', 'Transfer', '1002', 3000),
                            ('1001', '2022-06-26', 'Transfer', '1002', 4000)";

        using var connection = new SqlConnection(_accountConnStr);
        connection.Open();
        connection.Execute(insert);
    }


    private void Execute(string connStr, string query)
    {
        using var connection = new SqlConnection(connStr);
        connection.Open();
        connection.Execute(query);
    }



    // Dispose methods
    public void Dispose()
    {
        DisposeAccount();
        DisposeClient();
        DisposeAgency();
        DisposeEmployee();
        DisposeAddresses();
    }


    public void DisposeAddresses()
    {
        var client = new MongoClient("mongodb://root:Mongo%402024%23@localhost:27017");
        var database = client.GetDatabase("DbAddressUp");

        var collection = database.GetCollection<Address>("Address");

        var idsToDelete = new List<string>
        {
            "667c565b1573a059d574ce3d",
            "667c56b81573a059d574ce3e",
            "667c5983793d71b722f6cf3a",
            "667c599f793d71b722f6cf3b",
            "667c5a44f543f96f343fd5ef"
        };

        collection.DeleteMany(a => idsToDelete.Contains(a.Id));
    }


    public void DisposeAccount()
    {
        string deleteTransactions = @"DELETE FROM [DBAccountUpBank].[dbo].[AccountTransaction] WHERE AccountNumber IN (1001, 1002, 1003);";

        string deleteClientAccount = @"DELETE FROM [DBAccountUpBank].[dbo].[ClientAccount] WHERE AccountNumber IN (1001, 1002, 1003);";

        string deleteAccount = @"DELETE FROM [DBAccountUpBank].[dbo].[Account] WHERE AccountNumber IN (1001, 1002, 1003);";

        string deleteCreditCard = @"DELETE FROM [DBAccountUpBank].[dbo].[CreditCard] WHERE CreditCardNumber IN (5518593462108232, 5142473035102760, 5478254356487202);";

        using var connection = new SqlConnection(_accountConnStr);

        connection.Open();
        connection.Execute(deleteTransactions);
        connection.Execute(deleteClientAccount);
        connection.Execute(deleteAccount);
        connection.Execute(deleteCreditCard);
    }


    public void DisposeClient()
    {
        string deleteClient = @"
                DELETE FROM [UpBankClientAPI].[dbo].[Clients] 
                WHERE CPF IN ('64783003076', '65317260086', '12541068042', '71928364052', '39284712069')";

        Execute(_clientConnStr, deleteClient);
    }


    public void DisposeAgency()
    {
        string deleteEmployeeAgency = @"DELETE FROM [UpBank.AgencyAPI].[dbo].[Employee] WHERE CPF = '31143524055'";
        string deleteAgency = @"DELETE FROM [UpBank.AgencyAPI].[dbo].[Agency] WHERE Number = '4415'";

        Execute(_agencyConnStr, deleteAgency);
        Execute(_agencyConnStr, deleteEmployeeAgency);
    }


    public void DisposeEmployee()
    {
        string deleteEmployee = @"DELETE FROM [DBEmployee].[dbo].[Table_Employee] WHERE CPF = '31143524055'";

        Execute(_employeeConnStr, deleteEmployee);
    }

}
