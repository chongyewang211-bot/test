using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace test.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("key")]
        public string Key { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        
        [BsonElement("icon")]
        public string Icon { get; set; } = string.Empty;
        
        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;
        
        [BsonElement("problemCount")]
        public int ProblemCount { get; set; }
        
        [BsonElement("easyCount")]
        public int EasyCount { get; set; }
        
        [BsonElement("mediumCount")]
        public int MediumCount { get; set; }
        
        [BsonElement("hardCount")]
        public int HardCount { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
