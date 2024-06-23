using Models.Bank;

namespace Models.DTO
{
    public class TransactionDTO
    {
        public string AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string ReceiverNumber { get; set; }
        public double TransactionValue { get; set; }
    }
}