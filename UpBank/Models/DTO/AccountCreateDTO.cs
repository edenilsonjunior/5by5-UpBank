using Models.Bank;

namespace Models.DTO
{
    public class AccountCreateDTO
    {
        public int EmployeeRegister { get; set; }
        public string AccountNumber { get; set; }
        public string AgencyNumber { get; set; }
        public string AccountProfile { get; set; }
        public List<string> ClientCPF { get; set; }
        public double Overdraft { get; set; }
        public double CreditCardLimit { get; set; }
        public string CreditCardHolder { get; set; }
    }
}
