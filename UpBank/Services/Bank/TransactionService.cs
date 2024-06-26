using Dapper;
using Microsoft.Data.SqlClient;
using Models.Bank;
using Models.DTO;
using Repositories;
using Repositories.Utils;

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
            List<TransactionDTO> transactionDTO = await _repository.GetAllTransactions();
            if (transactionDTO.Count == 0) throw new ArgumentNullException("Nao existe nenhuma transacao efetuada");

            foreach (var dto in transactionDTO)
            {
                ETransactionType type = Enum.TryParse<ETransactionType>(dto.TransactionType, out var transactionType) ? transactionType : default;

                BankTransaction bt = new()
                {
                    Id = dto.Id,
                    TransactionDt = dto.TransactionDt,
                    Type = type,
                    Value = dto.TransactionValue
                };

                if (dto.ReceiverAccount != null)
                {
                    var account = await _accountService.GetAccount(dto.ReceiverAccount);
                    bt.Receiver = account.Number;
                }

                transactions.Add(bt);
            }
            return transactions;
        }

        public async Task<BankTransaction> GetTransaction(int Id)
        {
            TransactionDTO transactionDTO = await _repository.GetTransaction(Id);
            if (transactionDTO == null) throw new ArgumentNullException("O identificador da transacao nao existe.");

            Account account = await _accountService.GetAccount(transactionDTO.ReceiverAccount);
            if (account == null) throw new ArgumentNullException("A conta nao existe.");
            var type = Enum.TryParse<ETransactionType>(transactionDTO.TransactionType, out var transactionType) ? transactionType : default;

            return new BankTransaction
            {
                Id = transactionDTO.Id,
                Receiver = account.Number,
                TransactionDt = transactionDTO.TransactionDt,
                Type = type,
                Value = transactionDTO.TransactionValue
            };
        }

        public async Task<List<BankTransaction>> GetTransactionByType(string Type)
        {
            List<BankTransaction> transactions = new();
            List<TransactionDTO> transactionDTO = await _repository.GetTransactionsByType(Type);
            if (transactionDTO.Count == 0) return new();

            foreach (var dto in transactionDTO)
            {
                var type = Enum.TryParse<ETransactionType>(dto.TransactionType, out var transactionType) ? transactionType : default;

                BankTransaction bt = new()
                {
                    Id = dto.Id,
                    TransactionDt = dto.TransactionDt,
                    Type = type,
                    Value = dto.TransactionValue,
                    Receiver = dto.ReceiverAccount
                };

                transactions.Add(bt);
            }
            return transactions;
        }

        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            bool UseOverdraft = false;
            double diff = 0.0;
            Account account = _accountService.GetAccount(transactionDTO.AccountNumber).Result;
            double totalBalance = account.Balance + account.Overdraft;
            Account receiver = new();

            if (account.Number.Equals(transactionDTO.ReceiverAccount)) throw new InvalidOperationException("Nao pode ter pagamento ou transferencia para si mesmo.");

            if (transactionDTO.ReceiverAccount != null)
            {
                receiver = _accountService.GetAccount(transactionDTO.ReceiverAccount).Result;
                if (receiver == null) throw new ArgumentNullException("A conta destino nao existe.");
            }

            ETransactionType type = Enum.TryParse<ETransactionType>(transactionDTO.TransactionType, out var transactionType) ? transactionType : default;

            object obj = null;
            string bankTransaction = null;

            switch (transactionType.ToString())
            {
                case "Lending":
                case "Deposit":

                    if (transactionDTO.ReceiverAccount != null) throw new InvalidOperationException("Nao pode ter conta destino para emprestimo ou deposito.");
                    obj = new
                    {
                        Value = transactionDTO.TransactionValue,
                        AccountNumber = transactionDTO.AccountNumber
                    };
                    bankTransaction = BankTransaction.UPDATEBALANCE;
                    break;

                case "Withdraw":

                    if (transactionDTO.ReceiverAccount != null) throw new InvalidOperationException("Nao pode ter conta destino para saque");
                    if (transactionDTO.TransactionValue > account.Balance)
                    {
                        double amountToRemove;

                        if (account.Balance >= 0)
                        {
                            diff = transactionDTO.TransactionValue - account.Balance;
                            amountToRemove = diff;
                            diff *= -1;
                        }
                        else
                        {
                            diff = transactionDTO.TransactionValue * -1;
                            diff += account.Balance;
                            amountToRemove = transactionDTO.TransactionValue;
                        }

                        if (amountToRemove > account.Overdraft) throw new InvalidOperationException("Saldo insuficiente para realizar o saque");

                        obj = new
                        {
                            Value = transactionDTO.TransactionValue,
                            AccountNumber = transactionDTO.AccountNumber,
                            Diff = diff,
                            UpdateOverdraft = amountToRemove
                        };
                        bankTransaction = BankTransaction.UPDATEBALANCEWITHDRAWOVERDRAFT;
                    }
                    else
                    {
                        obj = new
                        {
                            Value = transactionDTO.TransactionValue,
                            AccountNumber = transactionDTO.AccountNumber
                        };
                        bankTransaction = BankTransaction.UPDATEBALANCEWITHDRAW;
                    }

                    break;
                case "Transfer":
                case "Payment":

                    double amountToRemoveOverdraft = 0.0;

                    if (transactionDTO.ReceiverAccount == null)
                        throw new InvalidOperationException("Nao foi informado a conta destino para transferencia ou pagamento");


                    if (transactionDTO.TransactionValue > account.Balance)
                    {
                        UseOverdraft = true;


                        if (account.Balance >= 0)
                        {
                            diff = transactionDTO.TransactionValue - account.Balance; // O novo saldo da conta vai ser o valor da transacao menos o saldo atual
                            amountToRemoveOverdraft = diff; // O valor que vai ser retirado do cheque especial vai ser o valor da diferenca
                            diff *= -1;
                        }
                        else
                        {
                            // O saldo novo vai ser a trasacao negativada e o saldo atual (exemplo: -10 + -100 = -110)
                            diff = transactionDTO.TransactionValue * -1;
                            diff += account.Balance;
                            amountToRemoveOverdraft = transactionDTO.TransactionValue; // O valor que vai ser retirado do cheque especial vai ser o valor da transacao
                        }

                        if (amountToRemoveOverdraft > account.Overdraft)
                        {
                            throw new InvalidOperationException("Saldo insuficiente para realizar o pagamento ou transferencia");
                        }
                    };

                    if (UseOverdraft)
                    {
                        obj = new
                        {
                            Value = transactionDTO.TransactionValue,
                            AccountNumber = transactionDTO.AccountNumber,
                            Diff = diff,
                            ReceiverAccount = transactionDTO.ReceiverAccount,
                            UpdateOverdraft = amountToRemoveOverdraft
                        };
                        bankTransaction = BankTransaction.UPDATEBALANCEOVERDRAFT;
                    }
                    else
                    {
                        //Se o receiver estiver com valor negativo, ele usou o cheque especial. Ao entrar uma transferencia na conta, primeiro completara o cheque para depois ir para o saldo
                        if (receiver.Balance < 0)
                        {
                            //Verifica qual o menor valor entre o saldo do destino e o valor da transacao e retorna ele.
                            double overdraftCovered = Math.Min(-receiver.Balance, transactionDTO.TransactionValue);
                            double totalCoveredAmount = receiver.Balance + overdraftCovered;
                            double remainingAmount = transactionDTO.TransactionValue - overdraftCovered;
                            obj = new
                            {
                                Value = remainingAmount + totalCoveredAmount,
                                Diff = overdraftCovered,
                                AccountNumber = transactionDTO.AccountNumber,
                                ReceiverAccount = transactionDTO.ReceiverAccount
                            };
                            bankTransaction = BankTransaction.UPDATEBALANCERECEIVEROVERDRAFT;
                        }
                        else
                        {
                            obj = new
                            {
                                Value = transactionDTO.TransactionValue,
                                AccountNumber = transactionDTO.AccountNumber,
                                ReceiverAccount = transactionDTO.ReceiverAccount
                            };
                            bankTransaction = BankTransaction.UPDATEBALANCERECEIVER;
                        }
                    }

                    break;
                default:
                    throw new ArgumentException("Nao existe esse tipo de transacao");
            }

            DapperUtilsRepository<BankTransaction>.Insert(bankTransaction, obj);
            BankTransaction transaction = _repository.InsertTransaction(transactionDTO).Result;

            if (transactionDTO.ReceiverAccount != null)
                transaction.Receiver = receiver.Number;

            return transaction;
        }
    }
}