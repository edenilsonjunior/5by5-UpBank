using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Models.DTO;
using Models.Bank;
using System.Net;
using Models.People;
using Services.Utils;
using System.Security.AccessControl;

namespace Services.Bank
{
    public class AccountService
    {
        private AccountRepository _repository;

        private string _agenciesUri = "https://localhost:7166";
        private string _clientsUri = "https://localhost:7166";


        public AccountService()
        {
            _repository = new();
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            List<AccountDTO> dtoList = await _repository.GetAllAccounts();

            var list = new List<Account>();

            foreach (var register in dtoList)
            {
                Account a = new()
                {
                    Number = register.AccountNumber,
                    Restriction = register.Restriction,
                    CreditCard = register.CreditCard,
                    Overdraft = register.Overdraft,
                    Profile = EnumConvert<EProfile>.Parse(register.AccountProfile),
                    Balance = register.Balance,
                    CreatedDt = register.CreatedDt,
                    Client = new()
                };

                var agency = await ApiConsume<Agency>.Get(_agenciesUri, $"/GetAgencies/{register.AgencyNumber}");

                a.Agency = agency ?? throw new ArgumentException("Agencia não encontrada");

                List<string> cpfs = await _repository.GetClientsCpfsByAccountNumber(a.Number);

                List<Task<Client>> tasks = new();

                foreach (var c in cpfs)
                {
                    tasks.Add(ApiConsume<Client>.Get(_clientsUri, $"/GetClients/{c}"));
                }

                Task.WaitAll(tasks.ToArray());

                foreach (var task in tasks)
                {
                    Client? client = task.Result ?? throw new ArgumentException("Cliente não encontrado");

                    a.Client.Add(client);
                }

                a.Extract = await _repository.GetTransactionsByNumber(a.Number);
                list.Add(a);
            }

            return list;
        }

        public async Task<Account> GetAccount(string number)
        {
            AccountDTO register = await _repository.GetAccount(number);

            Account account = new()
            {
                Number = register.AccountNumber,
                Restriction = register.Restriction,
                CreditCard = register.CreditCard,
                Overdraft = register.Overdraft,
                Profile = EnumConvert<EProfile>.Parse(register.AccountProfile),
                Balance = register.Balance,
                CreatedDt = register.CreatedDt,
                Client = new()
            };

            var agency = await ApiConsume<Agency>.Get(_agenciesUri, $"/GetAgencies/{register.AgencyNumber}");

            account.Agency = agency ?? throw new ArgumentException("Agencia não encontrada");

            List<string> cpfs = await _repository.GetClientsCpfsByAccountNumber(account.Number);

            List<Task<Client>> tasks = new();

            foreach (var c in cpfs)
            {
                tasks.Add(ApiConsume<Client>.Get(_clientsUri, $"/GetClients/{c}"));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                Client? client = task.Result ?? throw new ArgumentException("Cliente não encontrado");

                account.Client.Add(client);
            }

            account.Extract = await _repository.GetTransactionsByNumber(account.Number);

            return account;
        }

        public async Task<Account> CreateAccount(AccountDTO accountDTO)
        {

            if (accountDTO.ClientCPF == null || accountDTO.ClientCPF.Count == 0 || accountDTO.ClientCPF.Count > 2)
                throw new ArgumentException("A lista de clientes é inválida");

            Account account = new(accountDTO);

            var clientsTask = accountDTO.ClientCPF.Select(cpf => ApiConsume<Client>.Get(_clientsUri, $"/GetClients/{cpf}")).ToList();
            var agencyTask = ApiConsume<Agency>.Get(_agenciesUri, $"/GetAgencies/{accountDTO.AgencyNumber}");

            await Task.WhenAll(clientsTask);

            foreach (var task in clientsTask)
            {
                Client? client = task.Result ?? throw new ArgumentException($"Cliente não encontrado");

                account.Client.Add(client);
            }

            account.Agency = agencyTask.Result ?? throw new ArgumentException("Agencia não encontrada");


            if (account.Client[0].BirthDt.AddYears(18) > DateTime.Now)
                throw new ArgumentException("O dono da conta deve ser maior de idade");

            foreach (var c in account.Client)
            {
                if (c.Restriction == true)
                    throw new ArgumentException($"Cliente {c.Name} com restrição");
            }


            try
            {
                return _repository.PostAccount(account);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
