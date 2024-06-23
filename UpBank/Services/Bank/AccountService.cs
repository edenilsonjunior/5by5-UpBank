using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Models.DTO;
using Models.Bank;

namespace Services.Bank
{
    public class AccountService
    {
        private AccountRepository _repository;


        public AccountService()
        {
            _repository = new();
            
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            var a = await _repository.GetAllAccounts();

            throw new NotImplementedException();
        }

        public async Task<Account> GetAccount(string number)
        {
            var a = await _repository.GetAccount(number);

            throw new NotImplementedException();
        }

        public async Task<Account> CreateAccount(Account account)
        {
            var a = await _repository.PostAccount(account);

            throw new NotImplementedException();
        }

    }
}
