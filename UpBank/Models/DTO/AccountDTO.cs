namespace Models.DTO
{
    public class AccountDTO
    {
        public string AgencyNumber { get; set; }
        public string AccountNumber { get; set; }
        public List<string> ClientCPF { get; set; }
        public bool Restriction { get; set; }
        public long CreditCardNumber { get; set; }
        public double Overdraft { get; set; }
        public int ProfileId { get; set; }
        public DateTime CreatedDt { get; set; }
        public double Balance { get; set; }
        public List<int> ExtractId { get; set; }
    }
}
