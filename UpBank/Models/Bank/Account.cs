using Models.DTO;
using Models.People;

namespace Models.Bank
{
    public class Account
    {
        public static readonly string Get = @"
        SELECT 
            a.AccountNumber,
            a.AgencyNumber,
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


        public static readonly string INSERT = @"INSERT INTO Account(AccountNumber, AgencyNumber, AccountProfile,  Restriction, CreditCardNumber, Overdraft, CreatedDt, Balance) VALUES (@AccountNumber, @AgencyNumber, @AccountProfile, @Restriction, @CreditCardNumber, @Overdraft, @CreatedDt, @Balance)";

        public static readonly string DELETE = @"INSERT INTO AccountHistory(AccountNumber, AgencyNumber, AccountProfile,  Restriction, CreditCardNumber, Overdraft, CreatedDt, Balance) VALUES (@AccountNumber, @AgencyNumber, @AccountProfile, @Restriction, @CreditCardNumber, @Overdraft, @CreatedDt, @Balance);DELETE FROM Account WHERE AccountNumber = @Number";

        public string Number { get; set; }
        public Agency Agency { get; set; }
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
            Number = accountDTO.AccountNumber;
            CreditCard = accountDTO.CreditCard;

            Overdraft = 0;
            Balance = 0;
            CreatedDt = DateTime.Now;
            Restriction = true;
            Client = new();
            Extract = new();
            Profile = EProfile.Normal;
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

