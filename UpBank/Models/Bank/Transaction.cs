    namespace Models.Bank
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime TransactionDt { get; set; }
        public ETransactionType Type { get; set; }
        public Account Receiver { get; set; }
        public double Value { get; set; }
    }
}