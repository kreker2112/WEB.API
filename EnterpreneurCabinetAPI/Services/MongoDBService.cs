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

            // Коллекция для транзакций
            _transactions = database.GetCollection<Transactions>("Transactions");

            // Коллекция для пользователей
            _users = database.GetCollection<User>("Users");
        }

        // Методы для работы с транзакциями
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

        // Методы для работы с пользователями
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
            // Получаем пользователя по его UserID
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            // Проверяем, что пользователь существует и имеет данные по доходам
            if (user != null && user.IncomeReceipts.Any())
            {
                // Извлекаем все уникальные значения годов
                var years = user.IncomeReceipts.Select(receipt => receipt.Year).Distinct().ToList();
                return years;
            }

            // Если данных нет, возвращаем пустой список
            return new List<int>();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            newUser.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<List<Quarter>?> GetReceiptsByYearAsync(string userId, int year)
        {
            // Находим пользователя по UserID
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            // Проверяем, существует ли пользователь и есть ли у него данные о доходах
            if (user != null)
            {
                // Ищем данные по конкретному году
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    // Возвращаем список кварталов с поступлениями
                    return receiptsForYear.Quarters;
                }
            }

            // Если данные не найдены, возвращаем null
            return null;
        }

        public async Task<List<string>?> GetReceiptsByYearAndQuarterAsync(string userId, int year, string quarter)
        {
            // Находим пользователя по UserID
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            // Проверяем, существует ли пользователь и есть ли у него данные о доходах
            if (user != null)
            {
                // Ищем данные за указанный год
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    // Ищем данные за указанный квартал и возвращаем только массив поступлений
                    var receiptsForQuarter = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarter);
                    return receiptsForQuarter?.Receipts;
                }
            }

            // Если данных нет, возвращаем null
            return null;
        }

        public async Task<List<string>?> GetReceiptsForMultipleQuartersAsync(string userId, int year, int numberOfQuarters)
        {
            // Находим пользователя по UserID
            var user = await _users.Find(u => u.UserID == userId).FirstOrDefaultAsync();

            // Проверяем, существует ли пользователь и есть ли у него данные о доходах
            if (user != null)
            {
                // Ищем данные за указанный год
                var receiptsForYear = user.IncomeReceipts.FirstOrDefault(r => r.Year == year);
                if (receiptsForYear != null)
                {
                    // Список для хранения всех квитанций
                    List<string> allReceipts = new List<string>();

                    // Выбираем данные по кварталам
                    for (int i = 0; i < numberOfQuarters; i++)
                    {
                        var quarterName = $"Q{i + 1}"; // Формируем имя квартала (Q1, Q2, Q3, Q4)
                        var receiptsForQuarter = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarterName);

                        if (receiptsForQuarter != null)
                        {
                            allReceipts.AddRange(receiptsForQuarter.Receipts); // Добавляем квитанции за квартал
                        }
                    }

                    return allReceipts;
                }
            }

            // Если данных нет, возвращаем null
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
                    receiptsForYear = new Receipt { Year = year };
                    user.IncomeReceipts.Add(receiptsForYear);
                }

                var quarterReceipts = receiptsForYear.Quarters.FirstOrDefault(q => q.QuarterName == quarter);
                if (quarterReceipts == null)
                {
                    quarterReceipts = new Quarter { QuarterName = quarter };
                    receiptsForYear.Quarters.Add(quarterReceipts);
                }

                quarterReceipts.Receipts.Add(newReceipt);

                var result = await _users.ReplaceOneAsync(u => u.UserID == user.UserID, user);
                return result.ModifiedCount > 0;
            }
            return false;
        }
    }
}
