using Models.Bank;

namespace Models.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public DateTime TransactionDt { get; set; } = DateTime.Now;
        public string TransactionType { get; set; }
        public string? ReceiverNumber { get; set; }
        public double TransactionValue { get; set; }
    }
}