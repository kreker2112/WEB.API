using Microsoft.AspNetCore.Mvc;
using EnterpreneurCabinetAPI.Services;

namespace EnterpreneurCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(MongoDBService mongoDBService) : ControllerBase
    {
        private readonly MongoDBService _mongoDBService = mongoDBService;

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUserIDs()
        {
            var userIds = await _mongoDBService.GetAllUserIDsAsync();
            if (userIds.Count == 0)
                return NotFound("No users found.");

            return Ok(userIds);
        }

        [HttpGet("{userId}/years")]
        public async Task<IActionResult> GetYearsForUser(string userId)
        {
            var years = await _mongoDBService.GetYearsForUserAsync(userId);

            if (years == null || years.Count == 0)
                return NotFound("No years found for this user or user does not exist");

            return Ok(years);
        }

        [HttpGet("{userId}/receipts")]
        public async Task<IActionResult> GetReceiptsByYear(string userId, [FromQuery] int year)
        {
            var quarters = await _mongoDBService.GetReceiptsByYearAsync(userId, year);

            if (quarters == null || quarters.Count == 0)
                return NotFound("No receipts found for the specified year or user does not exist");

            return Ok(quarters);
        }

        [HttpGet("{userId}/quarters")]
        public async Task<IActionResult> GetQuartersByYear(string userId, [FromQuery] int year)
        {
            var quarters = await _mongoDBService.GetQuartersByYearAsync(userId, year);

            if (quarters == null || quarters.Count==0)
                return NotFound("No quarters found for the specified year or user does not exist");

            return Ok(quarters);
        }

        [HttpGet("{userId}/receipts/specific")]
        public async Task<IActionResult> GetReceiptsByYearAndQuarter(string userId, [FromQuery] int year, [FromQuery] string quarter)
        {
            var receipts = await _mongoDBService.GetReceiptsByYearAndQuarterAsync(userId, year, quarter);

            if (receipts == null || receipts.Count == 0)
                return NotFound($"No receipts found for the specified year {year} and quarter {quarter}");

            return Ok(receipts);
        }

        [HttpGet("{userId}/receipts/quarters/Q1")]
        public async Task<IActionResult> GetReceiptsForFirstQuarter(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 1);

            if (receipts == null || receipts.Count == 0)
                return NotFound("No receipts found for the specified year and first quarter");

            return Ok(receipts);
        }

        [HttpGet("{userId}/receipts/quarters/Q2")]
        public async Task<IActionResult> GetReceiptsForFirstAndSecondQuarter(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 2);

            if (receipts == null || receipts.Count == 0)
                return NotFound("No receipts found for the specified year and first two quarters");

            return Ok(receipts);
        }

        [HttpGet("{userId}/receipts/quarters/Q3")]
        public async Task<IActionResult> GetReceiptsForFirstThreeQuarters(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 3);

            if (receipts == null || receipts.Count == 0)
                return NotFound("No receipts found for the specified year and first three quarters");

            return Ok(receipts);
        }

        [HttpGet("{userId}/receipts/quarters/Q4")]
        public async Task<IActionResult> GetReceiptsForAllFourQuarters(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 4);

            if (receipts == null || receipts.Count == 0)
                return NotFound("No receipts found for the specified year and all four quarters");

            return Ok(receipts);
        }

        [HttpGet("{userId}/receipts/before-quarter")]
        public async Task<IActionResult> GetReceiptsBeforeQuarter(string userId, [FromQuery] int year, [FromQuery] string quarter)
        {
            var receipts = await _mongoDBService.GetReceiptsBeforeQuarterAsync(userId, year, quarter);

            if (receipts == null || receipts.Count == 0)
                return NotFound($"No receipts found for user {userId}, year {year}, before quarter {quarter}");

            return Ok(receipts);
        }

        [HttpPost("{userId}/receipts")]
        public async Task<IActionResult> AddReceipt(string userId, [FromQuery] int year, [FromQuery] string quarter, [FromBody] string newReceipt)
        {
            var result = await _mongoDBService.AddReceiptAsync(userId, year, quarter, newReceipt);
            if (!result)
                return BadRequest("Failed to add receipt");

            return Ok("Receipt added successfully");
        }

        [HttpGet("{userId}/tax/details")]
        public async Task<IActionResult> GetTaxPaymentDetails(string userId)
        {
            var taxDetails = await _mongoDBService.GetTaxPaymentDetailsAsync(userId);

            if (taxDetails == null)
                return NotFound($"Tax payment details not found for user {userId}");

            return Ok(taxDetails);
        }
    }
}
