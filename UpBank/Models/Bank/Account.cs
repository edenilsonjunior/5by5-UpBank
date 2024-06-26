using Models.DTO;
using Models.People;
using System;
using System.Security.Principal;

namespace Models.Bank
{
    public class Account
    {
        public static readonly string Get = @"
        SELECT 
            a.AccountNumber,
            a.AgencyNumber,
            a.SavingAccountNumber,
            a.Restriction,
            a.CreditCardNumber,
            a.Overdraft,
            a.AccountProfile,
            a.CreatedDt AS AccountCreatedDt,
            a.Balance,
            cc.ExpirationDt,
            cc.CreditCardLimit,
            cc.Cvv,
            cc.Holder,
            cc.Flag
        FROM 
            Account a
        JOIN 
            CreditCard cc ON a.CreditCardNumber = cc.CreditCardNumber";

        public static readonly string GetByNumber = Get + " WHERE a.AccountNumber = @Number";

        public static readonly string GetByClientCPF = @"SELECT ClientCPF FROM ClientAccount WHERE AccountNumber = @AccountNumber";

        public static readonly string INSERT = @"INSERT INTO Account(AccountNumber, AgencyNumber, SavingAccountNumber, AccountProfile,  Restriction, CreditCardNumber, Overdraft, CreatedDt, Balance) VALUES (@AccountNumber, @AgencyNumber, @SavingAccountNumber, @AccountProfile, @Restriction, @CreditCardNumber, @Overdraft, @CreatedDt, @Balance)";

        public static readonly string UPDATE = @"
        UPDATE Account SET Restriction = @Restriction, Overdraft = @Overdraft WHERE AccountNumber = @AccountNumber;
        UPDATE Account SET AccountProfile = @AccountProfile WHERE AccountNumber = @AccountNumber;
        UPDATE CreditCard SET Active = @Active, CreditCardLimit = @CreditCardLimit WHERE CreditCardNumber = @CreditCardNumber";

        public static readonly string DELETE = @"INSERT INTO AccountHistory(AccountNumber, AgencyNumber, SavingAccountNumber, AccountProfile, Restriction, CreditCardNumber, Overdraft, CreatedDt, Balance) VALUES (@AccountNumber, @AgencyNumber, @SavingAccountNumber, @AccountProfile, @Restriction, @CreditCardNumber, @Overdraft, @CreatedDt, @Balance);UPDATE Account SET Restriction = 1 WHERE AccountNumber = @AccountNumber";

        private static readonly string[] cardBrands = { "Visa", "MasterCard", "American Express", "Elo" };

        private static string FlagRandom { get => cardBrands[new Random().Next(cardBrands.Length)]; }


        public string Number { get; set; }
        public Agency Agency { get; set; }
        public string SavingAccountNumber { get; set; }
        public List<Client> Client { get; set; }
        public bool Restriction { get; set; }
        public CreditCard CreditCard { get; set; }
        public double Overdraft { get; set; }
        public EProfile Profile { get; set; }
        public DateTime CreatedDt { get; set; }
        public double Balance { get; set; }
        public List<BankTransaction> Extract { get; set; }


        public Account() { }

        public Account(AccountDTO accountDTO)
        {
            Random r = new();

            Number = accountDTO.AccountNumber;
            SavingAccountNumber = $"{Number}-{r.Next(10, 100)}";
            Restriction = true;
            Overdraft = accountDTO.Overdraft;
            Profile = Enum.Parse<EProfile>(accountDTO.AccountProfile);
            CreatedDt = DateTime.Now;
            Balance = 0;

            Client = new();
            Extract = new();

            CreditCard = new()
            {
                Number = r.NextInt64(1000000000000000, 9999999999999999),
                ExpirationDt = DateTime.Now.AddYears(5),
                Limit = accountDTO.CreditCardLimit,
                CVV = r.Next(100, 999).ToString(),
                Holder = accountDTO.CreditCardHolder,
                Flag = FlagRandom,
                Active = false
            };
        }

        public Account(AccountDTO dto, Agency agency, List<Client> clients, List<BankTransaction> extract)
        {
            Number = dto.AccountNumber;
            Restriction = dto.Restriction;
            CreditCard = dto.CreditCard;
            Overdraft = dto.Overdraft;
            Balance = dto.Balance;
            CreatedDt = dto.CreatedDt;

            Client = clients;
            Agency = agency;
            Extract = extract;

            if (Enum.TryParse<EProfile>(dto.AccountProfile, true, out var result))
                Profile = result;   
        }
    }
}