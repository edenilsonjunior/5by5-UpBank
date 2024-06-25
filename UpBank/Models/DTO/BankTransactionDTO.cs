using Models.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class BankTransactionDTO
    {
        public int Id { get; set; }
        public DateTime TransactionDt { get; set; }
        public ETransactionType Type { get; set; }
        public string AccountReceiver { get; set; }
        public double Value { get; set; }

        public BankTransactionDTO() { }

        public BankTransactionDTO(dynamic data)
        {
            Id = data.Id;
            TransactionDt = data.TransactionDt;
            Enum.TryParse<ETransactionType>(data.TransactionType, true, out ETransactionType type);
            AccountReceiver = data.ReceiverAccount;
            Type = type;
            Value = data.TransactionValue;
        }
    }
}
