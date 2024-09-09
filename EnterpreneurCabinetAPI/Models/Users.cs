using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EnterpreneurCabinetAPI.Models
{
    public class Receipt
    {
        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("quarters")]
        public List<Quarter> Quarters { get; set; } = new();
    }

    public class Quarter
    {
        [BsonElement("quarter")]
        public string QuarterName { get; set; } = string.Empty;

        [BsonElement("receipts")]
        public List<string> Receipts { get; set; } = new();
    }

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserID")]
        public required string UserID { get; set; }

        [BsonElement("UserName")]
        public string? UserName { get; set; }

        [BsonElement("DepartmentName")]
        public string? DepartmentName { get; set; }

        [BsonElement("DateOfJoining")]
        public string? DateOfJoining { get; set; }

        [BsonElement("PhotoFileName")]
        public string? PhotoFileName { get; set; }

        [BsonElement("PersonalTaxId")]
        public string? PersonalTaxId { get; set; }

        [BsonElement("RegistrationAddress")]
        public string? RegistrationAddress { get; set; }

        [BsonElement("IncomeReceipts")]
        public List<Receipt> IncomeReceipts { get; set; } = new();
    }
}
