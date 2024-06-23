using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.DTO;
using Services.Bank;

namespace UpBank.AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private AccountService _accountService;

        public AccountsController()
        {
            _accountService = new();
        }

        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpGet("{number}")]
        public async Task<ActionResult<Account>> GetAccount(string number)
        {
            var account = await _accountService.GetAccount(number);
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(AccountDTO accountDTO)
        {

            var newAccount = await _accountService.CreateAccount(accountDTO);
            return Ok(newAccount);
        }
    }
}
