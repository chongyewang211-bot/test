using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace test.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;
        
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
        
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}

