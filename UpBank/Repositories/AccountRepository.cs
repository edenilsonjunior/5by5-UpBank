using Dapper;
using Microsoft.Data.SqlClient;
using Models.Bank;
using Models.DTO;
using Models.People;
using Repositories.Utils;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Repositories
{
    public class AccountRepository
    {
        public AccountRepository() { }

        public async Task<List<AccountDTO>> GetAllAccounts()
        {
            var registers = DapperUtilsRepository<dynamic>.GetAll(Account.Get);

            var accounts = new List<AccountDTO>();
            foreach (var row in registers)
            {
                accounts.Add(new AccountDTO(row));
            }
            return accounts;
        }

        public async Task<AccountDTO> GetAccount(string number)
        {
            var register = DapperUtilsRepository<dynamic>.Get(Account.GetByNumber, new { Number = number });

            return new AccountDTO(register);
        }

        public async Task<List<BankTransaction>> GetTransactions(string number)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetClientsCpfsByAccountNumber(string number)
        {
            return DapperUtilsRepository<string>.GetAll(Account.GetByClientCPF, new { AccountNumber = number });
        }



        public Account PostAccount(Account account)
        {
            try
            {
                object cardObj = new
                {
                    CreditCardNumber = account.CreditCard.Number,
                    ExpirationDt = account.CreditCard.ExpirationDt,
                    CreditCardLimit = account.CreditCard.Limit,
                    Cvv = account.CreditCard.CVV,
                    Holder = account.CreditCard.Holder,
                    Flag = account.CreditCard.Flag
                };

                DapperUtilsRepository<Account>.Insert(CreditCard.Insert, cardObj);


                object accountObj = new
                {
                    AccountNumber = account.Number,
                    AgencyNumber = account.Agency.Number,
                    Restriction = account.Restriction,
                    CreditCardNumber = account.CreditCard.Number,
                    Overdraft = account.Overdraft,
                    CreatedDt = account.CreatedDt,
                    Balance = account.Balance,
                    AccountProfile = account.Profile.ToString()
                };

                DapperUtilsRepository<Account>.Insert(Account.INSERT, accountObj);

                foreach (var client in account.Client)
                {
                    object c = new
                    {
                        AccountNumber = account.Number,
                        ClientCPF = client.CPF
                    };
                    DapperUtilsRepository<Account>.Insert(Client.InsertClientAccount, c);
                }

                return account;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteAccount(Account account)
        {
            throw new NotImplementedException();
        }
    }
}