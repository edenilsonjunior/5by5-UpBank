namespace Models.Bank
{
    public class CreditCard
    {
        public long Number { get; set; }
        public DateTime ExpirationDt { get; set; }
        public double Limit { get; set; }
        public string CVV { get; set; }
        public string Holder { get; set; }
        public string Flag { get; set; }
    }
}
