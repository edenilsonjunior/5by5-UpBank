namespace Models.DTO
{
    public class AccountUpdateDTO
    {
        public string Number { get; set; }
        public bool Restriction { get; set; }
        public bool CreditCardStatus { get; set; }
        public string? AccountProfile { get; set; }
    }
}