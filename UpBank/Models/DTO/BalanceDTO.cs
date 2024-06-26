using Models.Bank;

namespace Models.DTO;

public class BalanceDTO
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public double Balance { get; set; }
    public double Overdraft { get; set; }
    public double CreditCardLimit { get; set; }

    public BalanceDTO() { }

    public BalanceDTO(Account account)
    {
        AccountNumber = account.Number;
        OwnerName = account.Client[0].Name;
        Balance = account.Balance;
        Overdraft = account.Overdraft;
        CreditCardLimit = account.CreditCard.Limit;
    }
}
