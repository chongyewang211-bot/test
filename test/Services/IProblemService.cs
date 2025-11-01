using test.Models;

namespace test.Services
{
    public interface IProblemService
    {
        Task<List<Problem>> GetAllProblemsAsync();
        Task<List<Problem>> GetProblemsByCategoryAsync(string category);
        Task<Problem?> GetProblemByIdAsync(string id);
        Task<Problem> CreateProblemAsync(Problem problem);
        Task<Problem> UpdateProblemAsync(string id, Problem problem);
        Task<bool> DeleteProblemAsync(string id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task SeedDataAsync();
    }
}
