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

        [HttpPost]
        public async Task<BankTransaction> InsertTransaction(TransactionDTO transactionDTO)
        {
            return await _transactionService.InsertTransaction(transactionDTO);
        }
    }
}