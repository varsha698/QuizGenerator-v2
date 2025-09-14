using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuizGeneratorApi.Api.Models;
public class Quiz
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("questions")]
    public List<Question> Questions { get; set; }

    [BsonElement("category")]
    public string Category { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; }

    [BsonElement("difficulty")]
    public string Difficulty { get; set; }

    [BsonElement("timeLimit")]
    public int? TimeLimit { get; set; }

    [BsonElement("points")]
    public int Points { get; set; }

    [BsonElement("stats")]
    public Stats Stats { get; set; }

    [BsonElement("createdBy")]
    public string CreatedBy { get; set; }

    [BsonElement("isPublic")]
    public bool IsPublic { get; set; }

    [BsonElement("isFeatured")]
    public bool IsFeatured { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("__v")]
    public int Version { get; set; }
}

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("question")]
    public string Text { get; set; }

    [BsonElement("options")]
    public List<string> Options { get; set; }

    [BsonElement("correctAnswer")]
    public int CorrectAnswer { get; set; }
}

public class Stats
{
    [BsonElement("totalAttempts")]
    public int TotalAttempts { get; set; }

    [BsonElement("averageScore")]
    public double AverageScore { get; set; }

    [BsonElement("totalTime")]
    public double TotalTime { get; set; }

    [BsonElement("averageRating")]
    public double AverageRating { get; set; }

    [BsonElement("ratings")]
    public List<int> Ratings { get; set; }
}




