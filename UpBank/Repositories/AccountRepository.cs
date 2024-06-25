using Models.Bank;
using Models.DTO;
using Models.People;
using Repositories.Utils;

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

            if (register == null)
                return null;

            return new AccountDTO(register);
        }

        public async Task<List<BankTransactionDTO>> GetTransactionsByNumber(string number)
        {
            var registers = DapperUtilsRepository<dynamic>.GetAll(BankTransaction.GetByAccount, new { AccountNumber = number });

            var transactions = new List<BankTransactionDTO>();
            foreach (var row in registers)
            {

                BankTransactionDTO btDTO = new(row);
                transactions.Add(btDTO);
            }

            return transactions;
        }

        public async Task<List<string>> GetClientsCpfsByAccountNumber(string number)
        {
            return DapperUtilsRepository<string>.GetAll(Account.GetByClientCPF, new { AccountNumber = number });
        }


        public async Task<bool> ApproveAccount(Account account)
        {
            string query = "update Account set Restriction = 0 where AccountNumber = @AccountNumber";
            return DapperUtilsRepository<Account>.Insert(query, new { AccountNumber = account.Number });
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
                    Flag = account.CreditCard.Flag,
                    Active = account.CreditCard.Active
                };

                try
                {
                    DapperUtilsRepository<Account>.Insert(CreditCard.Insert, cardObj);
                }
                catch (Exception) { throw new InvalidOperationException("Erro ao inserir: O cartao de crédito pertence a outra conta"); }


                object accountObj = new
                {
                    AccountNumber = account.Number,
                    AgencyNumber = account.Agency.Number,
                    SavingAccountNumber = account.SavingAccountNumber,
                    Restriction = account.Restriction,
                    CreditCardNumber = account.CreditCard.Number,
                    Overdraft = account.Overdraft,
                    CreatedDt = account.CreatedDt,
                    Balance = account.Balance,
                    AccountProfile = account.Profile.ToString()
                };

                try
                {
                    DapperUtilsRepository<Account>.Insert(Account.INSERT, accountObj);
                }
                catch (Exception) { throw new InvalidOperationException("Erro ao inserir. O numero de conta digitado pertence a outra conta"); }


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

        public Account PostAccountHistory(Account account)
        {
            try
            {
                object accountObj = new
                {
                    AccountNumber = account.Number,
                    AgencyNumber = account.Agency.Number,
                    SavingAccountNumber = account.SavingAccountNumber,
                    Restriction = account.Restriction,
                    CreditCardNumber = account.CreditCard.Number,
                    Overdraft = account.Overdraft,
                    CreatedDt = account.CreatedDt,
                    Balance = account.Balance,
                    AccountProfile = account.Profile.ToString(),
                };

                DapperUtilsRepository<Account>.Insert(Account.DELETE, accountObj);

                return account;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

