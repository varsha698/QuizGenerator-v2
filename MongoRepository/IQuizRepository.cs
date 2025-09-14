using QuizGeneratorApi.Api.Models;

namespace QuizGeneratorApi.Api.MongoRepository;

public interface IQuizRepository
{
    Task<(IReadOnlyList<Quiz> Items, long Total)> GetAsync(int page, int pageSize, string? search = null, string? category = null);
    Task<Quiz?> GetByIdAsync(string id);
    Task<string> CreateAsync(Quiz quiz);
    Task<bool> UpdateAsync(string id, Quiz quiz);
    Task<bool> DeleteAsync(string id);

    // Question-level ops
    Task<bool> AddQuestionAsync(string quizId, Question question);
    Task<bool> UpdateQuestionAsync(string quizId, string questionId, Question question);
    Task<bool> RemoveQuestionAsync(string quizId, string questionId);

    // Optional: ensure indexes once
    Task EnsureIndexesAsync();
}
