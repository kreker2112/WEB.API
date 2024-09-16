using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EnterpreneurCabinetAPI.Models
{
    public class Receipt
    {
        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("quarters")]
        public List<Quarter> Quarters { get; set; } = [];
    }

    public class Quarter
    {
        [BsonElement("quarter")]
        public string QuarterName { get; set; } = string.Empty;

        [BsonElement("receipts")]
        public List<string> Receipts { get; set; } = [];
    }

    public class TaxPaymentDetails
    {
        [BsonElement("RecipientEDRPOU")]
        public string RecipientEDRPOU { get; set; } = string.Empty;

        [BsonElement("RecipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [BsonElement("Account")]
        public string Account { get; set; } = string.Empty;

        [BsonElement("AccountName")]
        public string AccountName { get; set; } = string.Empty;

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
        public List<Receipt> IncomeReceipts { get; set; } = [];

        [BsonElement("TaxPaymentDetails")]
        public TaxPaymentDetails? TaxPaymentDetails { get; set; }
    }
}
