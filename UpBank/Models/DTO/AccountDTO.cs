using Models.Bank;
using System.Text.Json.Serialization;

namespace Models.DTO
{
    public class AccountDTO
    {
        public string AccountNumber { get; set; }
        public string AgencyNumber { get; set; }
        public string AccountProfile { get; set; }
        public List<string> ClientCPF { get; set; }
        public double Overdraft { get; set; }
        public double CreditCardLimit { get; set; }
        public string CreditCardHolder { get; set; }


        [JsonIgnore]
        public CreditCard? CreditCard { get; set; }

        [JsonIgnore]
        public DateTime CreatedDt { get; set; }

        [JsonIgnore]
        public bool Restriction { get; set; }

        [JsonIgnore]
        public double Balance { get; set; }

        [JsonIgnore]
        public string? SavingAccountNumber { get; set; }

        public AccountDTO() { }

        public AccountDTO(dynamic row)
        {
            AccountNumber = row.AccountNumber;
            AgencyNumber = row.AgencyNumber;
            Restriction = row.Restriction;
            Overdraft = row.Overdraft;
            AccountProfile = row.AccountProfile;
            CreatedDt = row.AccountCreatedDt;
            Balance = row.Balance;

            CreditCard = new CreditCard
            {
                Number = row.CreditCardNumber,
                ExpirationDt = row.ExpirationDt,
                Limit = row.CreditCardLimit,
                CVV = row.Cvv,
                Holder = row.Holder,
                Flag = row.Flag
            };

            SavingAccountNumber = row.SavingAccountNumber;
        }
    }
}
