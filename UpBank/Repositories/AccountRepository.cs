using Dapper;
using Microsoft.Data.SqlClient;
using Models.Bank;
using Models.DTO;

namespace Repositories
{
    public class AccountRepository
    {
        private string Conn { get; set; }

        public AccountRepository()
        {
            Conn = "Data Source = 127.0.0.1; Initial Catalog=DBAccountUpBank; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=True;";
        }

        public async Task<List<AccountDTO>> GetAllAccounts()
        {
            List<AccountDTO> accounts = new();

            try
            {
                using (var db = new SqlConnection(Conn))
                {
                    db.Open();
                    var query = await db.QueryAsync<AccountDTO>(Account.GETALL);
                    foreach (var account in query)
                    {
                        accounts.Add(account);
                    }
                    db.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return accounts;
        }

        public async Task<AccountDTO> GetAccount(string number)
        {
            AccountDTO account = new();

            try
            {
                using (var db = new SqlConnection(Conn))
                {
                    db.Open();
                    var query = await db.QueryAsync<AccountDTO>(Account.GET, new { number });

                    account = query.FirstOrDefault();

                    db.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return account;
        }

        public async Task<Account> PostAccount(Account account)
        {
            int result = 0;
            try
            {
                using (var db = new SqlConnection(Conn))
                {
                    db.Open();
                    result = db.ExecuteScalar<int>(Account.INSERT, new
                    {
                        Number = account.Number,
                        AgencyNumber = account.Agency.Number,
                        ClientCpf = account.Client,
                        Restriction = account.Restriction,
                        CreditCardNumber = account.CreditCard.Number,
                        Overdraft = account.Overdraft,
                        AccountProfile = account.Profile,
                        CreatedDt = account.CreatedDt,
                        Balance = account.Balance,
                        ExtractId = account.Extract
                    });
                    db.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<int> DeleteAccount(Account account)
        {
            int result = 0;
            try
            {
                using (var db = new SqlConnection(Conn))
                {
                    db.Open();
                    result = db.ExecuteScalar<int>(Account.DELETE, new
                    {
                        Number = account.Number,
                        AgencyNumber = account.Agency.Number,
                        ClientCpf = account.Client,
                        Restriction = account.Restriction,
                        CreditCardNumber = account.CreditCard.Number,
                        Overdraft = account.Overdraft,
                        AccountProfile = account.Profile,
                        CreatedDt = account.CreatedDt,
                        Balance = account.Balance,
                        ExtractId = account.Extract
                    });
                    db.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}