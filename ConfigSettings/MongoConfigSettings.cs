namespace QuizGeneratorApi.Api.ConfigSettings;

public class MongoConfigSettings
{
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string QuizzesCollectionName { get; set; } = "Quizzes";
}
