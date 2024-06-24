using Dapper;
using Microsoft.Data.SqlClient;
using Models.Bank;
using Models.DTO;
using Repositories;
using System.Data.Common;
using System.Transactions;

namespace Services.Bank
{
    public class TransactionService
    {
        private TransactionRepository _repository;
        private AccountService _accountService;

        public TransactionService()
        {
            _repository = new();
            _accountService = new();
        }

        public async Task<List<BankTransaction>> GetAllTransactions()
        {
            List<BankTransaction> transactions = new();
            var transactionDTO = await _repository.GetAllTransactions();
            if (transactionDTO.Count == 0) throw new ArgumentNullException("Nao existe nenhuma transacao efetuada");

            foreach (var dto in transactionDTO)
            {
                var account = await _accountService.GetAccount(dto.AccountNumber);
                var type = Enum.TryParse<ETransactionType>(dto.TransactionType, out var transactionType) ? transactionType : default;

                transactions.Add(new BankTransaction
                {
                    Id = dto.Id,
                    Receiver = account,
                    TransactionDt = dto.TransactionDt,
                    Type = type,
                    Value = dto.TransactionValue
                });
            }
            return transactions;
        }

        public async Task<BankTransaction> GetTransaction(int Id)
        {
            var transactionDTO = await _repository.GetTransaction(Id);
            if (transactionDTO == null) throw new ArgumentNullException("O identificador da transacao nao existe.");

            var account = await _accountService.GetAccount(transactionDTO.AccountNumber);
            var type = Enum.TryParse<ETransactionType>(transactionDTO.TransactionType, out var transactionType) ? transactionType : default;

            return new BankTransaction
            {
                Id = transactionDTO.Id,
                Receiver = account,
                TransactionDt = transactionDTO.TransactionDt,
                Type = type,
                Value = transactionDTO.TransactionValue
            };
        }

        public async Task<List<BankTransaction>> GetTransactionByType(string Type)
        {
            List<BankTransaction> transactions = new();
            var transactionDTO = await _repository.GetTransactionsByType(Type);
            if (transactionDTO.Count == 0) throw new ArgumentNullException("Nao existe nenhuma transacao efetuada");

            foreach (var dto in transactionDTO)
            {
                var account = await _accountService.GetAccount(dto.AccountNumber);
                var type = Enum.TryParse<ETransactionType>(dto.TransactionType, out var transactionType) ? transactionType : default;

                transactions.Add(new BankTransaction
                {
                    Id = dto.Id,
                    Receiver = account,
                    TransactionDt = dto.TransactionDt,
                    Type = type,
                    Value = dto.TransactionValue
                });
            }
            return transactions;
        }

        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            Account account = new();
            //var account = _accountService.GetAccount(transactionDTO.AccountNumber).Result.Value;
            double totalBalance = account.Balance + account.Overdraft;
            AccountDTO receiver;
            var transaction = _repository.InsertTransaction(transactionDTO).Result;

            //if (transaction.Receiver.Number != null)
            //{
            //    receiver = await _accountService.GetAccount(transactionDTO.ReceiverNumber);
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