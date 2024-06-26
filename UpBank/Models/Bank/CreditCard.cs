using System.ComponentModel.DataAnnotations;

namespace Models.Bank
{
    public class CreditCard
    {
        public static readonly string Insert = @"INSERT INTO CreditCard VALUES (@CreditCardNumber, @ExpirationDt, @CreditCardLimit, @Cvv, @Holder, @Flag, @Active)";

        public long Number { get; set; }
        public DateTime ExpirationDt { get; set; }
        public double Limit { get; set; }
        public string CVV { get; set; }
        public string Holder { get; set; }
        public string Flag { get; set; }
        public bool Active { get; set; }
    }
}
