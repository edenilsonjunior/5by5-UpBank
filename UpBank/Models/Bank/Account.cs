using Models.People;
using System.ComponentModel.DataAnnotations;

namespace Models.Bank
{
    public class Account 
    {

        public Agency Agency { get; set; } 
        [Key]
        public string Number { get; set; }
        public List<Client> Client { get; set; }
        public bool Restriction { get; set; }
        public CreditCard CreditCard { get; set; }
        public double Overdraft { get; set; }
        public EProfile Profile { get; set; }
        public DateTime CreatedDt { get; set; }
        public double Balance { get; set; }
        public List<Transaction> Extract { get; set; }
        public List<Client> Clients { get; set; }
    }
}
