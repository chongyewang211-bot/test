using MongoDB.Driver;
using test.Configuration;
using test.Models;

namespace test.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentService(MongoDbService mongoDbService, MongoDbSettings settings)
        {
            _comments = mongoDbService.Database.GetCollection<Comment>("comments");
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _comments
                .Find(c => c.IsActive)
                .SortByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> CreateCommentAsync(string content, string username, string userId)
        {
            var comment = new Comment
            {
                Content = content,
                Username = username,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _comments.InsertOneAsync(comment);
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(string id)
        {
            var result = await _comments.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }
    }
}

