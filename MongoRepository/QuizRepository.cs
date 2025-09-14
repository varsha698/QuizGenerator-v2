using MongoDB.Bson;
using MongoDB.Driver;
using QuizGeneratorApi.Api.Models;

namespace QuizGeneratorApi.Api.MongoRepository;

public class QuizRepository : IQuizRepository
{
    private readonly IMongoCollection<Quiz> _quizzes;

    public QuizRepository(IMongoCollection<Quiz> quizzes)
    {
        _quizzes = quizzes;
    }

    public async Task EnsureIndexesAsync()
    {
        var keys = Builders<Quiz>.IndexKeys.Text(q => q.Name).Text(q => q.Description);
        await _quizzes.Indexes.CreateOneAsync(new CreateIndexModel<Quiz>(keys));

        var catIdx = Builders<Quiz>.IndexKeys.Ascending(q => q.Category);
        await _quizzes.Indexes.CreateOneAsync(new CreateIndexModel<Quiz>(catIdx));
    }

    public async Task<(IReadOnlyList<Quiz> Items, long Total)> GetAsync(int page, int pageSize, string? search = null, string? category = null)
    {
        var filter = Builders<Quiz>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Uses text index on Name/Description
            filter &= Builders<Quiz>.Filter.Text(search);
        }
        if (!string.IsNullOrWhiteSpace(category))
        {
            filter &= Builders<Quiz>.Filter.Eq(q => q.Category, category);
        }

        var total = await _quizzes.CountDocumentsAsync(filter);
        var items = await _quizzes
            .Find(filter)
            .SortByDescending(q => q.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Quiz?> GetByIdAsync(string id)
    {
        return await _quizzes.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(Quiz quiz)
    {
        quiz.Id = ObjectId.GenerateNewId().ToString();
        var now = DateTime.UtcNow;
        quiz.CreatedAt = now;
        quiz.UpdatedAt = now;
        await _quizzes.InsertOneAsync(quiz);
        return quiz.Id;
    }

    public async Task<bool> UpdateAsync(string id, Quiz quiz)
    {
        quiz.Id = id;                    // enforce route id
        quiz.UpdatedAt = DateTime.UtcNow;
        var result = await _quizzes.ReplaceOneAsync(q => q.Id == id, quiz);
        return result.MatchedCount > 0 && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _quizzes.DeleteOneAsync(q => q.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> AddQuestionAsync(string quizId, Question question)
    {
        question.Id = ObjectId.GenerateNewId().ToString();
        var update = Builders<Quiz>.Update
            .Push(q => q.Questions, question)
            .Set(q => q.UpdatedAt, DateTime.UtcNow);

        var result = await _quizzes.UpdateOneAsync(q => q.Id == quizId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateQuestionAsync(string quizId, string questionId, Question question)
    {
        // positional operator to update a specific question by id
        var filter = Builders<Quiz>.Filter.And(
            Builders<Quiz>.Filter.Eq(q => q.Id, quizId),
            Builders<Quiz>.Filter.ElemMatch(q => q.Questions, x => x.Id == questionId)
        );

        var update = Builders<Quiz>.Update
            .Set(q => q.Questions[-1].Text, question.Text)
            .Set(q => q.Questions[-1].Options, question.Options)
            .Set(q => q.Questions[-1].CorrectAnswer, question.CorrectAnswer)
            .Set(q => q.UpdatedAt, DateTime.UtcNow);

        var result = await _quizzes.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveQuestionAsync(string quizId, string questionId)
    {
        var update = Builders<Quiz>.Update
            .PullFilter(q => q.Questions, x => x.Id == questionId)
            .Set(q => q.UpdatedAt, DateTime.UtcNow);

        var result = await _quizzes.UpdateOneAsync(q => q.Id == quizId, update);
        return result.ModifiedCount > 0;
    }
}
