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
        private TransactionsController _transactionsController;

        public AccountsController()
        {
            _accountService = new();
            _transactionsController = new();
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

        [HttpGet("TransactionType/{type}")]
        public async Task<ActionResult<List<BankTransaction>>> GetTransactionByType(string type)
        {
            var transaction = await _transactionsController.GetTransactionsByType(type);
            if (transaction.Count == 0) return NotFound("Nao existem transacoes efetuadas deste tipo");
            return Ok(transaction);
        }

        [HttpPost("MakeTransaction")]
        public async Task<ActionResult<BankTransaction>> MakeTransaction(TransactionDTO transactionDTO)
        {
            var account = GetAccount(transactionDTO.AccountNumber).Result.Value;
            account.Extract = new List<BankTransaction>();
            if (account.Restriction) return BadRequest("Conta esta restrita e nao pode efetuar transacao");
            var transaction = await _transactionsController.InsertTransaction(transactionDTO);
            if (transaction == null) return BadRequest("Erro ao efetuar transacao");
            if (account == null) return NotFound($"Nao foi encontrada uma conta com o numero {transactionDTO.AccountNumber}");

            account.Extract.Add(transaction);

            return Ok(transaction);
        }

        //[HttpPatch]
        //public async Task<ActionResult<Account>> UpdateAccount(string number)
        //{
            //if (account.Restriction) return BadRequest("Conta esta restrita e nao pode ter seus dados atualizados");
        //}

        //[HttpDelete]
        //public async Task<ActionResult<Account>> CloseAccount(string number)
        //{

        //}
    }
}