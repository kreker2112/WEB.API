using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EnterpreneurCabinetAPI.Models
{
    public class Transactions
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("transaction")]
        public required string TransactionsDetail { get; set; }
    }

}
