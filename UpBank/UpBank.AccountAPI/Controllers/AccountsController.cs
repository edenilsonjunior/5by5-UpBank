using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.DTO;
using Services.Bank;
using System.Net.Http;
using System.Net;

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
        

        // Gets

        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccounts();
            return Ok(accounts);
        }


        [HttpGet("{number}")]
        public async Task<ActionResult<Account>> GetAccount(string number)
        {
            try
            {
                var account = await _accountService.GetAccount(number);
                return Ok(account);
            }
            catch (Exception) { return NotFound("Conta nao encontrada"); }
        }


        [HttpGet("TransactionType/{type}")]
        public async Task<ActionResult<List<BankTransaction>>> GetTransactionByType(string type)
        {
            var transaction = await _transactionsController.GetTransactionsByType(type);

            return transaction.Count == 0 ? NotFound("Nao existem transacoes efetuadas deste tipo") : Ok(transaction);
        }

        
        [HttpGet("GetBankStatement/{accountNumber}")]
        public async Task<ActionResult<List<BankTransaction>>> GetBankStatement(string accountNumber)
        {
            var account = _accountService.GetAccount(accountNumber).Result;

            if (account == null)
                return NotFound($"Nao foi encontrada uma conta com o numero {accountNumber}");

            var transactions = await _accountService.GetTransactionsByNumber(accountNumber);

            return Ok(transactions);
        }


        [HttpGet("GetBalance/{accountNumber}")]
        public async Task<ActionResult<BalanceDTO>> GetBalance(string accountNumber)
        {
            try
            {
                var account = _accountService.GetAccount(accountNumber).Result;
                BalanceDTO balance = new(account);

                return Ok(balance);
            }
            catch (Exception)
            {
                return NotFound($"Nao foi encontrada uma conta com o numero {accountNumber}");
            }
        }


        // Posts

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(AccountDTO accountDTO)
        {
            try
            {
                var newAccount = await _accountService.CreateAccount(accountDTO);
                return Ok(newAccount);
            }
            catch (NullReferenceException e) { return BadRequest(e.Message); }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (Exception e) { return StatusCode(500, e.Message); }
        }


        [HttpPost("MakeTransaction")]
        public async Task<ActionResult<BankTransaction>> MakeTransaction(TransactionDTO transactionDTO)
        {
            Account account = _accountService.GetAccount(transactionDTO.AccountNumber).Result;
            if (account == null) return NotFound($"Nao foi encontrada uma conta com o numero {transactionDTO.AccountNumber}");
            if (account.Restriction) return BadRequest("Conta esta restrita e nao pode efetuar transacao");
            //if(account.CreditCard.Active) return BadRequest("Cartao de credito esta restrito");
            var transaction = await _transactionsController.InsertTransaction(transactionDTO);
            if (transaction == null) return BadRequest("Erro ao efetuar transacao");

            return Ok(transaction);
        }


        // Patches

        [HttpPatch("ApproveAccount/{accountNumber}")]
        public async Task<ActionResult<Account>> ApproveAccount(string accountNumber)
        {
            var account = _accountService.GetAccount(accountNumber).Result;
            if (account == null)
                return NotFound($"Nao foi encontrada uma conta com o numero {accountNumber}");
            if (!account.Restriction)
                return BadRequest("Conta ja esta aprovada");

            account.Restriction = false;

            bool updatedAccount = await _accountService.ApproveAccount(account);

            if (!updatedAccount)
                return BadRequest("Erro ao aprovar conta");

            return Ok(account);
        }





        //[HttpPatch]
        //public async Task<ActionResult<Account>> UpdateAccount(string number)
        //{
        //if (account.Restriction) return BadRequest("Conta esta restrita e nao pode ter seus dados atualizados");
        //}

        //[HttpDelete]
        //public async Task<ActionResult<Account>> CloseAccount(string number)
        //{
        //    var account = _accountService.GetAccount(number).Result;

        //    _accountService.CreateAccount(account);
        //}
    }
}