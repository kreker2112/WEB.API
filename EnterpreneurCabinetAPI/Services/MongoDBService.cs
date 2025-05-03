using MongoDB.Driver;
using EnterpreneurCabinetAPI.Models;
using MongoDB.Bson;

namespace EnterpreneurCabinetAPI.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Transactions> _transactions;
        private readonly IMongoCollection<User> _users;

        public MongoDBService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);

            _transactions = database.GetCollection<Transactions>("Transactions");

            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<string>> GetTransactionDetailsAsync()
        {
            var projection = Builders<Transactions>.Projection.Expression(t => t.TransactionsDetail);
            var results = await _transactions.Find(transaction => true)
                                             .Project<string>(projection)
                                             .ToListAsync();
            return results;
        }

        public async Task<Transactions> PostAsync(Transactions transaction)
        {
            transaction.Id = ObjectId.GenerateNewId().ToString();
            await _transactions.InsertOneAsync(transaction);
            return transaction;
        }

        public async Task RemoveAllAsync()
        {
            await _transactions.DeleteManyAsync(transaction => true);
        }

        public async Task<List<string>> GetAllUserIDsAsync()
        {
            var projection = Builders<User>.Projection.Expression(user => user.UserID);
            var userIds = await _users.Find(user => true)
                                      .Project<string>(projection)
                                      .ToListAsync();
            return userIds;
        }

        public async Task<User?> GetUserByUserIDAsync(string userId)
        {
            return await _users.Find(user => user.UserID == userId).FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetYearsForUserAsync(string userId)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null && user.IncomeReceipts.Count > 0)
            {
                var years = user.IncomeReceipts.Select(receipt => receipt.Year).Distinct().ToList();
                return years;
            }

            return [];
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            newUser.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<TaxPaymentDetails?> GetTaxPaymentDetailsAsync(string userId)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();
            return user?.TaxPaymentDetails;
        }


        public async Task<List<Quarter>?> GetReceiptsByYearAsync(string userId, int year)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {

                    return receiptsForYear.Quarters;
                }
            }


            return null;
        }

        public async Task<List<string>?> GetQuartersByYearAsync(string userId, int year)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    return receiptsForYear.Quarters.Select(q => q.QuarterName).ToList();
                }
            }

            return null;
        }

        public async Task<List<string>?> GetReceiptsByYearAndQuarterAsync(string userId, int year, string quarter)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    var receiptsForQuarter = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarter);
                    return receiptsForQuarter?.Receipts;
                }
            }

            return null;
        }

        public async Task<List<string>?> GetReceiptsForMultipleQuartersAsync(string userId, int year, int numberOfQuarters)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    var allReceipts = new List<string>();

                    for (int i = 0; i < numberOfQuarters; i++)
                    {
                        var quarterName = $"Q{i + 1}";
                        var receiptsForQuarter = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarterName);

                        if (receiptsForQuarter != null)
                        {
                            allReceipts.AddRange(receiptsForQuarter.Receipts);
                        }
                    }

                    return allReceipts;
                }
            }

            return null;
        }

        public async Task<bool> AddReceiptAsync(string userId, int year, string quarter, string newReceipt)
        {
            var user = await _users.Find(user => user.UserID == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear == null)
                {
                    receiptsForYear = new Receipt { Year = year, Quarters = [] };
                    user.IncomeReceipts.Add(receiptsForYear);
                }

                var quarterReceipts = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarter);
                if (quarterReceipts == null)
                {
                    quarterReceipts = new Quarter { QuarterName = quarter, Receipts = [] };
                    receiptsForYear.Quarters.Add(quarterReceipts);
                }

                quarterReceipts.Receipts.Add(newReceipt);

                var result = await _users.ReplaceOneAsync(u => u.UserID == user.UserID, user);
                return result.ModifiedCount > 0;
            }
            return false;
        }

        public async Task<List<string>?> GetReceiptsBeforeQuarterAsync(string userId, int year, string quarter)
        {
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    var allReceipts = new List<string>();

                    if (!quarter.StartsWith("Q") || quarter.Length != 2 || !int.TryParse(quarter.AsSpan(1), out int targetQuarterNumber))
                        return null;

                    for (int i = 1; i < targetQuarterNumber; i++)
                    {
                        string currentQuarter = $"Q{i}";
                        var q = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == currentQuarter);
                        if (q != null)
                            allReceipts.AddRange(q.Receipts);
                    }

                    return allReceipts;
                }
            }

            return null;
        }
    }
}
