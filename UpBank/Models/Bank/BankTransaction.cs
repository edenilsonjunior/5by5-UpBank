namespace Models.Bank
{
    public class BankTransaction
    {
        public static readonly string INSERT = "INSERT INTO AccountTransaction (AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue) VALUES (@AccountNumber, @TransactionDt, @TransactionType, @ReceiverAccount, @TransactionValue);SELECT CAST(SCOPE_IDENTITY() AS INT)";

        public static readonly string UPDATEBALANCE = "UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @AccountNumber";

        public static readonly string UPDATEBALANCEWITHDRAW = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber";

        public static readonly string UPDATEBALANCEOVERDRAFT = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber;UPDATE Account SET Overdraft = Overdraft - @Diff WHERE AccountNumber = @AccountNumber;UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @ReceiverAccount";

        public static readonly string UPDATEBALANCERECEIVER = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber;UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @ReceiverAccount";

        public static readonly string UPDATEBALANCERECEIVEROVERDRAFT = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber;UPDATE Account SET Balance = @Value WHERE AccountNumber = @ReceiverNumber;UPDATE Account SET Overdraft = Overdraft + @Diff WHERE AccountNumber = @ReceiverNumber";

        public static readonly string GETALL = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction";

        public static readonly string GET = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction WHERE Id = @Id";

        public static readonly string GETBYTYPE = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction WHERE TransactionType = @Type";

        public static readonly string GetByAccount = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction where AccountNumber = @AccountNumber";


        public int Id { get; set; }
        public DateTime TransactionDt { get; set; }
        public ETransactionType Type { get; set; }
        public Account? Receiver { get; set; }
        public double Value { get; set; }

        public BankTransaction() { }

        public BankTransaction(dynamic data)
        {
            Id = data.Id;
            TransactionDt = data.TransactionDt;

            Enum.TryParse<ETransactionType>(data.TransactionType, true, out ETransactionType type);

            Type = type;
            Value = data.TransactionValue;
        }
    }
}