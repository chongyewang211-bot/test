using test.Models;

namespace test.Services
{
    public interface ICommentService
    {
        Task<List<Comment>> GetAllCommentsAsync();
        Task<Comment> CreateCommentAsync(string content, string username, string userId);
        Task<bool> DeleteCommentAsync(string id);
    }
}

