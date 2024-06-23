using Models.Bank;
using Models.DTO;
using Repositories.Utils;

namespace Repositories
{
    public class TransactionRepository
    {
        private string Conn { get; set; }

        public TransactionRepository()
        {
            Conn = "Data Source=127.0.0.1; Initial Catalog=DbAccountUpBank; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";
        }

        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            BankTransaction transaction = new();

            object obj = new
            {
                AccountNumber = transactionDTO.AccountNumber,
                TransactionDt = DateTime.Now,
                TransactionType = transactionDTO.TransactionType,
                ReceiverAccount = transactionDTO.ReceiverNumber,
                TransactionValue = transactionDTO.TransactionValue
            };

            transaction.TransactionDt = DateTime.Now;
            transaction.Value = transactionDTO.TransactionValue;
            if (Enum.TryParse<ETransactionType>(transactionDTO.TransactionType, out var transactionType)) transaction.Type = transactionType;
            transaction.Receiver = new Account { Number = transactionDTO.ReceiverNumber };
            transaction.Id = DapperUtilsRepository<BankTransaction>.InsertWithScalar(BankTransaction.INSERT, obj);


            return transaction;
        }
    }
}