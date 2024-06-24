using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.DTO;
using Repositories;
using Services.Bank;

namespace UpBank.AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private TransactionService _transactionService;

        public TransactionsController()
        {
            _transactionService = new();
        }

        [HttpGet]
        public async Task<List<BankTransaction>> GetAllTransactions() => await _transactionService.GetAllTransactions();

        [HttpGet("Id/{id}")]
        public async Task<BankTransaction> GetTransaction(int id) => await _transactionService.GetTransaction(id);

        [HttpGet("Type/{type}")]
        public async Task<List<BankTransaction>> GetTransactionsByType(string type) => await _transactionService.GetTransactionByType(type);


        [HttpPost]
        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            return await _transactionService.InsertTransaction(transactionDTO);
        }
    }
}