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

        public async Task<List<TransactionDTO>> GetAllTransactions() => DapperUtilsRepository<TransactionDTO>.GetAll(BankTransaction.GETALL);

        public async Task<TransactionDTO> GetTransaction(int Id) => DapperUtilsRepository<TransactionDTO>.Get(BankTransaction.GET, new { Id });

        public async Task<List<TransactionDTO>> GetTransactionsByType(string Type) => DapperUtilsRepository<TransactionDTO>.GetAll(BankTransaction.GETBYTYPE, new { Type });

        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            BankTransaction transaction = new();

            object obj = new
            {
                AccountNumber = transactionDTO.AccountNumber,
                TransactionDt = DateTime.Now,
                TransactionType = transactionDTO.TransactionType,
                ReceiverAccount = transactionDTO.ReceiverAccount,
                TransactionValue = transactionDTO.TransactionValue
            };

            transaction.TransactionDt = DateTime.Now;
            transaction.Value = transactionDTO.TransactionValue;
            if (Enum.TryParse<ETransactionType>(transactionDTO.TransactionType, out var transactionType)) transaction.Type = transactionType;
            transaction.Id = DapperUtilsRepository<BankTransaction>.InsertWithScalar(BankTransaction.INSERT, obj);


            return transaction;
        }
    }
}