﻿using Dapper;
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
            var transactionDTO = await _repository.GetAllTransactions();
            if (transactionDTO.Count == 0) throw new ArgumentNullException("Nao existe nenhuma transacao efetuada");

            foreach (var dto in transactionDTO)
            {
                var account = await _accountService.GetAccount(dto.ReceiverNumber);
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

            var account = await _accountService.GetAccount(transactionDTO.ReceiverNumber);
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
                var account = await _accountService.GetAccount(dto.ReceiverNumber);
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
            bool UseOverdraft = false;
            double diff = 0.0;
            var account = _accountService.GetAccount(transactionDTO.AccountNumber).Result;
            double totalBalance = account.Balance + account.Overdraft;
            Account receiver;

            if (transactionDTO.ReceiverNumber != null)
            {
                receiver = _accountService.GetAccount(transactionDTO.ReceiverNumber).Result;
                if (receiver == null) throw new ArgumentNullException("A conta destino nao existe.");
            }

            if (Enum.TryParse<ETransactionType>(transactionDTO.TransactionType.ToString(), out var transactionType))
            {
                switch (transactionType)
                {
                    case ETransactionType.Withdraw:
                        if (transactionDTO.ReceiverNumber != null) throw new InvalidOperationException("Nao pode ter conta destino para saque");
                        if (transactionDTO.TransactionValue > totalBalance) throw new InvalidOperationException("Saldo insuficiente para realizar o saque");
                        break;
                    case ETransactionType.Deposit:
                    case ETransactionType.Lending:
                        if (transactionDTO.ReceiverNumber != null) throw new InvalidOperationException("Nao pode ter conta destino para emprestimo ou deposito.");
                        break;
                    case ETransactionType.Payment:
                    case ETransactionType.Transfer:
                        if (transactionDTO.TransactionValue > account.Balance)
                        {
                            UseOverdraft = true;
                            diff = transactionDTO.TransactionValue - account.Balance;

                            if (diff > account.Overdraft)
                            {
                                throw new InvalidOperationException("Saldo insuficiente para realizar o pagamento ou transferencia");
                            }

                            transactionDTO.TransactionValue = totalBalance;
                        };
                        if (transactionDTO.ReceiverNumber == null) throw new InvalidOperationException("Transferencia ou pagamento devem ter conta destino.");
                        break;
                }
            }
            else
            {
                throw new ArgumentException("Nao existe esse tipo de transacao");
            }

            var transaction = _repository.InsertTransaction(transactionDTO).Result;

            object obj = null;
            string bankTransaction = null;

            switch (transactionType.ToString())
            {
                case "Lending":
                case "Deposit":
                    obj = new
                    {
                        Value = transactionDTO.TransactionValue,
                        AccountNumber = transactionDTO.AccountNumber
                    };
                    bankTransaction = BankTransaction.UPDATEBALANCE;
                    break;
                case "Withdraw":
                    obj = new
                    {
                        Value = transactionDTO.TransactionValue,
                        AccountNumber = transactionDTO.AccountNumber
                    };
                    bankTransaction = BankTransaction.UPDATEBALANCEWITHDRAW;
                    break;
                case "Transfer":
                case "Payment":
                    if (UseOverdraft)
                    {
                        obj = new
                        {
                            Value = transactionDTO.TransactionValue,
                            AccountNumber = transactionDTO.AccountNumber,
                            Diff = diff,
                            ReceiverNumber = transactionDTO.ReceiverNumber
                        };
                        bankTransaction = BankTransaction.UPDATEBALANCEOVERDRAFT;
                    }
                    else
                    {
                        obj = new
                        {
                            Value = transactionDTO.TransactionValue,
                            AccountNumber = transactionDTO.AccountNumber,
                            ReceiverNumber = transactionDTO.ReceiverNumber
                        };
                        bankTransaction = BankTransaction.UPDATEBALANCERECEIVER;
                    }
                    break;
            }

            DapperUtilsRepository<BankTransaction>.Insert(bankTransaction, obj);

            return transaction;
        }

    }
}