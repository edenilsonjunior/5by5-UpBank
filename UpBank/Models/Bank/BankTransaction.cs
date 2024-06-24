namespace Models.Bank
{
    public class BankTransaction
    {
        public static readonly string INSERT = "INSERT INTO AccountTransaction (AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue) VALUES (@AccountNumber, @TransactionDt, @TransactionType, @ReceiverAccount, @TransactionValue);SELECT CAST(SCOPE_IDENTITY() AS INT)";

        public static readonly string UPDATEBALANCE = "UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @AccountNumber";

        public static readonly string UPDATEBALANCEWITHDRAW = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber";

        public static readonly string UPDATEBALANCEOVERDRAFT = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber;UPDATE Account SET Overdraft = Overdraft - @Diff WHERE AccountNumber = @AccountNumber;UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @ReceiverNumber";

        public static readonly string UPDATEBALANCERECEIVER = "UPDATE Account SET Balance = Balance - @Value WHERE AccountNumber = @AccountNumber;UPDATE Account SET Balance = Balance + @Value WHERE AccountNumber = @ReceiverNumber";

        public static readonly string GETALL = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction";

        public static readonly string GET = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction WHERE Id = @Id";

        public static readonly string GETBYTYPE = "SELECT Id, AccountNumber, TransactionDt, TransactionType, ReceiverAccount, TransactionValue FROM AccountTransaction WHERE TransactionType = @Type";

        public int Id { get; set; }
        public DateTime TransactionDt { get; set; }
        public ETransactionType Type { get; set; }
        public Account? Receiver { get; set; }
        public double Value { get; set; }
    }
}