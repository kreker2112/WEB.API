using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EnterpreneurCabinetAPI.Models;


namespace EnterpreneurCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(IMongoClient mongoClient) : ControllerBase
    {
        private readonly IMongoClient _mongoClient = mongoClient;

        // Getting all transactions details from the database 
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var collection = _mongoClient.GetDatabase("mypetprojectsdb").GetCollection<Transactions>("Transactions");
            var details = await collection.Find(Builders<Transactions>.Filter.Empty)
                                          .Project(t => t.TransactionsDetail)
                                          .ToListAsync();
            return new JsonResult(details);
        }

        // Adding a new transaction to the database
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Transactions transaction)
        {
            await _mongoClient.GetDatabase("mypetprojectsdb").GetCollection<Transactions>("Transactions").InsertOneAsync(transaction);
            return new JsonResult("AddedSuccessfully");
        }

        // Delete all transactions from the database
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await _mongoClient.GetDatabase("mypetprojectsdb").GetCollection<Transactions>("Transactions").DeleteManyAsync(transaction => true);
            return new JsonResult("DeletedSuccessfully");
        }
    }
}

