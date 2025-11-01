using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace test.Models
{
    public class Problem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("problemNumber")]
        public int ProblemNumber { get; set; }
        
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        
        [BsonElement("difficulty")]
        public string Difficulty { get; set; } = string.Empty; // Easy, Medium, Hard
        
        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;
        
        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new List<string>();
        
        [BsonElement("acceptanceRate")]
        public double AcceptanceRate { get; set; }
        
        [BsonElement("likes")]
        public int Likes { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
