namespace Models.Bank
{
    public class BankTransaction
    {
        public static readonly string Get = "select Id, TransactionDt, TransactionType, ReceiverAccount, TransactionValue from AccountTransaction";

        public static readonly string GetByAccount = Get + " where AccountNumber = @AccountNumber";

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