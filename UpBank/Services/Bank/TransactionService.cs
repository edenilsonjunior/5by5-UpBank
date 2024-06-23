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
            Account account = new Account();
            AccountDTO receiver;
            var list = account.Extract = new List<BankTransaction>();
            var transaction = _repository.InsertTransaction(transactionDTO).Result;
            if (transactionDTO.ReceiverNumber == "") transaction.Receiver = null;

            if (transaction.Receiver != null)
            {
                receiver = await _accountRepository.GetAccount(transactionDTO.ReceiverNumber);
                if (receiver == null) throw new ArgumentNullException("A conta destino nao existe.");
            }

            if (Enum.TryParse<ETransactionType>(transaction.Type.ToString(), out var transactionType))
            {
                switch (transactionType)
                {
                    case ETransactionType.Withdraw:
                        account.Balance -= transaction.Value;
                        break;
                    case ETransactionType.Deposit:
                    case ETransactionType.Lending:
                        account.Balance += transaction.Value;
                        break;
                    case ETransactionType.Payment:
                    case ETransactionType.Transfer:
                        if (transaction.Receiver == null)
                        {
                            throw new ArgumentNullException("Receiver nao pode ser null para transferencia ou pagamento");
                        }
                        account.Balance -= transaction.Value;
                        break;
                    default:
                        throw new ArgumentException("Nao existe esse tipo de transacao");
                }
            }

            list.Add(transaction);

            return transaction;
        }
    }
}