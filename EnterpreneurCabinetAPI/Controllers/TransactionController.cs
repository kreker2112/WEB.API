using Microsoft.AspNetCore.Mvc;
using EnterpreneurCabinetAPI.Services;
using EnterpreneurCabinetAPI.Models;

namespace EnterpreneurCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(MongoDBService mongoDBService) : ControllerBase
    {
        private readonly MongoDBService _mongoDBService = mongoDBService;

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactionDetails = await _mongoDBService.GetTransactionDetailsAsync();
            if (transactionDetails == null || transactionDetails.Count == 0)
                return NotFound("No transactions found.");

            return Ok(transactionDetails);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transactions transaction)
        {
            var addedTransaction = await _mongoDBService.PostAsync(transaction);
            return CreatedAtAction(nameof(GetAllTransactions), new { id = addedTransaction.Id }, addedTransaction);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllTransactions()
        {
            await _mongoDBService.RemoveAllAsync();
            return Ok("All transactions deleted successfully");
        }
    }
}
