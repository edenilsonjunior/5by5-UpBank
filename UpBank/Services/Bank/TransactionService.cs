using Dapper;
using Microsoft.Data.SqlClient;
using Models.Bank;
using Models.DTO;
using Repositories;

namespace Services.Bank
{
    public class TransactionService
    {
        private TransactionRepository _repository;
        private AccountRepository _accountRepository;

        public TransactionService()
        {
            _repository = new();
            _accountRepository = new();
        }

        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            Account account = new();
            //var account = _accountRepository.GetAccount(transactionDTO.AccountNumber).Result.Value;
            double totalBalance = account.Balance + account.Overdraft;
            AccountDTO receiver;
            var transaction = _repository.InsertTransaction(transactionDTO).Result;

            //if (transaction.Receiver.Number != null)
            //{
            //    receiver = await _accountRepository.GetAccount(transactionDTO.ReceiverNumber);
            //    if (receiver == null) throw new ArgumentNullException("A conta destino nao existe.");
            //}

            if (Enum.TryParse<ETransactionType>(transaction.Type.ToString(), out var transactionType))
            {
                switch (transactionType)
                {
                    case ETransactionType.Withdraw:
                        if (transaction.Value > totalBalance) throw new InvalidOperationException("Saldo insuficiente para realizar o saque");
                        account.Balance -= transaction.Value;
                        break;
                    case ETransactionType.Deposit:
                    case ETransactionType.Lending:
                        account.Balance += transaction.Value;
                        break;
                    case ETransactionType.Payment:
                    case ETransactionType.Transfer:
                        if (transaction.Value > totalBalance) throw new InvalidOperationException("Saldo insuficiente para realizar a transferencia");
                        if (transaction.Receiver == null) throw new ArgumentNullException("Receiver nao pode ser null para transferencia ou pagamento");
                        account.Balance -= transaction.Value;
                        break;
                    default:
                        throw new ArgumentException("Nao existe esse tipo de transacao");
                }
            }

            using (var db = new SqlConnection("Data Source=127.0.0.1; Initial Catalog=DbAccountUpBank; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes"))
            {
                if (transaction.Receiver.Number == null)
                {
                    await db.ExecuteAsync(BankTransaction.UPDATEBALANCE, new
                    {
                        Value = transaction.Value,
                        AccountNumber = transactionDTO.AccountNumber
                    });
                }
                else
                {
                    await db.ExecuteAsync(BankTransaction.UPDATEBALANCERECEIVER, new
                    {
                        Value = transaction.Value,
                        AccountNumber = transactionDTO.AccountNumber,
                        ReceiverNumber = transactionDTO.ReceiverNumber
                    });
                }
            }
            return transaction;
        }
    }
}