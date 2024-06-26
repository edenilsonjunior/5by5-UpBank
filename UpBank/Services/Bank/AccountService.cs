using Models.Bank;
using Models.DTO;
using Models.People;
using Repositories;
using Services.Utils;

namespace Services.Bank
{
    public class AccountService
    {
        private readonly AccountRepository _repository;
        private readonly string _agenciesUri;
        private readonly string _clientsUri;

        public AccountService()
        {
            _repository = new();
            _agenciesUri = "https://localhost:7217";
            _clientsUri = "https://localhost:7142";
        }


        public async Task<List<Account>> GetAllAccounts()
        {
            List<AccountDTO> dtoList = await _repository.GetAllAccounts();
            var list = new List<Account>();

            foreach (var register in dtoList)
            {
                var t1 = RetrieveAgency(register.AgencyNumber);
                var t2 = GetTransactionsByNumber(register.AccountNumber);

                var cpfs = await _repository.GetClientsCpfsByAccountNumber(register.AccountNumber);
                var t3 = RetrieveClients(cpfs);

                Agency agency = t1.Result;
                List<BankTransaction> extract = t2.Result;
                List<Client> clients = t3.Result;

                Account account = new(register, agency, clients, extract);
                account.SavingAccountNumber = register.SavingAccountNumber;

                list.Add(account);
            }

            return list;
        }


        public async Task<Account> GetAccount(string number)
        {
            AccountDTO register = await _repository.GetAccount(number);

            if (register == null)
                throw new ArgumentException("Conta não encontrada");

            var t1 = RetrieveAgency(register.AgencyNumber);
            var t2 = GetTransactionsByNumber(register.AccountNumber);

            var cpfs = await _repository.GetClientsCpfsByAccountNumber(register.AccountNumber);
            var t3 = RetrieveClients(cpfs);

            Agency agency = t1.Result;
            List<BankTransaction> extract = t2.Result;
            List<Client> clients = t3.Result;

            Account account = new(register, agency, clients, extract);
            account.SavingAccountNumber = register.SavingAccountNumber;

            return account;
        }


        public async Task<Account> CreateAccount(AccountDTO accountDTO)
        {
            try
            {
                ValidadeClientListDto(accountDTO.ClientCPF);

                Account account = new(accountDTO);

                var t1 = RetrieveClients(accountDTO.ClientCPF);
                var t2 = RetrieveAgency(accountDTO.AgencyNumber);

                await Task.WhenAll(t1, t2);

                account.Client = t1.Result;
                account.Agency = t2.Result;


                ValidadeAccount(account);
                return _repository.PostAccount(account);
            }
            catch (Exception) { throw; }
        }


        public async Task<bool> ApproveAccount(Account account)
        {
            return await _repository.ApproveAccount(account);
        }

        public async Task<Account> UpdateAccount(AccountUpdateDTO accountDTO)
        {
            var account = await GetAccount(accountDTO.Number);
            Random r = new();
            account.CreditCard.Active = accountDTO.CreditCardStatus;
            var profile = Enum.TryParse<EProfile>(accountDTO.AccountProfile, out var accountProfile) ? accountProfile : throw new ArgumentException("O perfil de conta informado nao existe.");
            account.Profile = profile;
            account.Restriction = accountDTO.Restriction;

            switch (account.Profile)
            {
                case EProfile.Academic:
                    account.CreditCard.Limit = r.Next(1000, 3001);
                    account.Overdraft = r.Next(500, 1501);
                    break;

                case EProfile.Normal:
                    account.CreditCard.Limit = r.Next(3000, 10001);
                    account.Overdraft = r.Next(1500, 5001);
                    break;

                case EProfile.VIP:
                    account.CreditCard.Limit = r.Next(10000, 50001);
                    account.Overdraft = r.Next(5000, 20001);
                    break;
            }

            await _repository.UpdateAccount(account);

            return account;
        }

        private async Task<List<Client>> RetrieveClients(List<string> cpfs)
        {
            var clientsTask = cpfs.Select(cpf => ApiConsume<Client>.Get(_clientsUri, $"api/Clients/{cpf}")).ToList();
            await Task.WhenAll(clientsTask);

            var list = new List<Client>();

            foreach (var task in clientsTask)
            {
                var client = task.Result ?? throw new ArgumentException($"Cliente não encontrado");
                list.Add(client);
            }

            return list;
        }


        private async Task<Agency> RetrieveAgency(string number)
        {
            var agency = await ApiConsume<Agency>.Get(_agenciesUri, $"api/Agencies/{number}");

            return agency ?? throw new ArgumentException("Agencia não encontrada");
        }


        private void ValidadeClientListDto(List<string> cpfs)
        {
            if (cpfs == null)
                throw new NullReferenceException("A lista de clientes é nula");

            if (cpfs.Count == 0 || cpfs.Count > 2)
                throw new ArgumentException("Deve conter pelo menos um cliente e no máximo 2");
        }



        public async Task<List<BankTransaction>> GetTransactionsByNumber(string number)
        {
            List<BankTransactionDTO> listDTO = await _repository.GetTransactionsByNumber(number);

            List<BankTransaction> list = new();

            foreach (var dto in listDTO)
            {
                BankTransaction bt = new()
                {
                    Id = dto.Id,
                    TransactionDt = dto.TransactionDt,
                    Type = dto.Type,
                    Value = dto.Value
                };

                bt.Receiver = dto.AccountReceiver;

                list.Add(bt);
            }

            return list;
        }

        private void ValidadeAccount(Account account)
        {
            List<Account> accounts = GetAllAccounts().Result;


            foreach (var ac in accounts)
            {
                if (ac.Number.Equals(account.Number))
                    throw new ArgumentException("Erro ao inserir. O numero de conta digitado pertence a outra conta");

                if (ac.CreditCard.Number == account.CreditCard.Number)
                    throw new ArgumentException("Erro ao inserir: O numero do cartao de crédito pertence a outra conta");

                if (ac.Client.Find(Client => Client.CPF.Equals(account.Client[0].CPF)) != null)
                    throw new ArgumentException("Erro ao inserir. O CPF digitado já pertence a outra conta");
            }

            if (account.Client[0].BirthDt.AddYears(18) > DateTime.Now)
                throw new ArgumentException("O dono da conta deve ser maior de idade");

            foreach (var c in account.Client)
            {
                if (c.Restriction == true)
                    throw new ArgumentException($"Cliente {c.Name} com restrição");
            }
        }

        public async Task<Account> DeleteAccount(string number)
        {
            var account = await GetAccount(number);
            if (account == null) throw new NullReferenceException($"A conta com o numero {number} nao consta na tabela Account");
            _repository.PostAccountHistory(account);

            return account;
        }
    }
}