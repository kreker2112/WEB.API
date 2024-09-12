using Microsoft.AspNetCore.Mvc;
using EnterpreneurCabinetAPI.Services;
using EnterpreneurCabinetAPI.Models;

namespace EnterpreneurCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public UsersController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // Получение всех пользователей
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUserIDs()
        {
            var userIds = await _mongoDBService.GetAllUserIDsAsync();
            return Ok(userIds);
        }

        // Получение данных пользователя по UserID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByUserID(string userId)
        {
            var user = await _mongoDBService.GetUserByUserIDAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpGet("{userId}/years")]
        public async Task<IActionResult> GetYearsForUser(string userId)
        {
            var years = await _mongoDBService.GetYearsForUserAsync(userId);

            if (years == null || !years.Any())
                return NotFound("No years found for this user or user does not exist");

            return Ok(years);
        }


        // Добавление нового пользователя
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User newUser)
        {
            var createdUser = await _mongoDBService.AddUserAsync(newUser);
            return CreatedAtAction(nameof(GetUserByUserID), new { userId = createdUser.UserID }, createdUser);
        }

        // Получение данных из receipts по году
        [HttpGet("{userId}/receipts")]
        public async Task<IActionResult> GetReceiptsByYear(string userId, [FromQuery] int year)
        {
            var quarters = await _mongoDBService.GetReceiptsByYearAsync(userId, year);

            if (quarters == null || !quarters.Any())
                return NotFound("No receipts found for the specified year or user does not exist");

            return Ok(quarters);
        }

        [HttpGet("{userId}/receipts/quarter")]
        public async Task<IActionResult> GetReceiptsByYearAndQuarter(string userId, [FromQuery] int year, [FromQuery] string quarter)
        {
            var receipts = await _mongoDBService.GetReceiptsByYearAndQuarterAsync(userId, year, quarter);

            if (receipts == null || !receipts.Any())
                return NotFound("No receipts found for the specified year and quarter, or user does not exist");

            return Ok(receipts);
        }

        // Получение квитанций за 1 квартал
        [HttpGet("{userId}/receipts/quarter/1")]
        public async Task<IActionResult> GetReceiptsForFirstQuarter(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 1);

            if (receipts == null || !receipts.Any())
                return NotFound("No receipts found for the specified year and first quarter");

            return Ok(receipts);
        }

        // Получение квитанций за 1 и 2 кварталы
        [HttpGet("{userId}/receipts/quarter/1and2")]
        public async Task<IActionResult> GetReceiptsForFirstAndSecondQuarter(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 2);

            if (receipts == null || !receipts.Any())
                return NotFound("No receipts found for the specified year and first two quarters");

            return Ok(receipts);
        }

        // Получение квитанций за 1, 2 и 3 кварталы
        [HttpGet("{userId}/receipts/quarter/1to3")]
        public async Task<IActionResult> GetReceiptsForFirstThreeQuarters(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 3);

            if (receipts == null || !receipts.Any())
                return NotFound("No receipts found for the specified year and first three quarters");

            return Ok(receipts);
        }

        // Получение квитанций за все 4 квартала
        [HttpGet("{userId}/receipts/quarter/1to4")]
        public async Task<IActionResult> GetReceiptsForAllFourQuarters(string userId, [FromQuery] int year)
        {
            var receipts = await _mongoDBService.GetReceiptsForMultipleQuartersAsync(userId, year, 4);

            if (receipts == null || !receipts.Any())
                return NotFound("No receipts found for the specified year and all four quarters");

            return Ok(receipts);
        }

        // Добавление новой строки в массив receipts
        [HttpPost("{userId}/receipts")]
        public async Task<IActionResult> AddReceipt(string userId, [FromQuery] int year, [FromQuery] string quarter, [FromBody] string newReceipt)
        {
            var result = await _mongoDBService.AddReceiptAsync(userId, year, quarter, newReceipt);
            if (!result)
                return BadRequest("Failed to add receipt");

            return Ok("Receipt added successfully");
        }
    }
}
