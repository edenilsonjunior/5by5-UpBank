using Models.People;

namespace Models.Bank
{
    public class Account
    {
        public static readonly string INSERT = "INSERT INTO Account VALUES (@Number, @AgencyNumber, @ClientCpf, @Restriction, @CreditCardNumber, @Overdraft, @AccountProfile, @CreatedDt, @Balance, @ExtractId)";

        public static readonly string GETALL = "SELECT FROM Account Number, AgencyNumber, ClientCPF, Restriction, CreditCardNumber, Overdraft, AccountProfile, CreatedDt, Balance, ExtractId";

        public static readonly string GET = "SELECT FROM Account Number, AgencyNumber, ClientCPF, Restriction, CreditCardNumber, Overdraft, AccountProfile, CreatedDt, Balance, ExtractId WHERE Number = @Number";

        public static readonly string DELETE = "INSERT INTO AccountHistory VALUES (@Number, @AgencyNumber, @ClientCPF, @Restriction, @CreditCardNumber, @Overdraft, @AccountProfile, @CreatedDt, @Balance, @ExtractId);DELETE FROM Account WHERE Number = @Number";

        public string Number { get; set; }
        public Agency Agency { get; set; }
        public List<Client> Client { get; set; }
        public bool Restriction { get; set; }
        public CreditCard CreditCard { get; set; }
        public double Overdraft { get; set; }
        public EProfile Profile { get; set; }
        public DateTime CreatedDt { get; set; }
        public double Balance { get; set; }
        public List<Transaction> Extract { get; set; }
    }
}
