using QuizGeneratorApi.Api.ConfigSettings;
using MongoDB.Driver; 
using QuizGeneratorApi.Api.Models;
using QuizGeneratorApi.Api.MongoRepository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoConfigSettings>(
    builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoConfigSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
// Add services to the container.
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongo = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoConfigSettings>>().Value;
    return mongo.GetDatabase(settings.DatabaseName);
});

// Register the collection and repository
builder.Services.AddSingleton<IMongoCollection<Quiz>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoConfigSettings>>().Value;
    return db.GetCollection<Quiz>(settings.QuizzesCollectionName);
});

builder.Services.AddScoped<IQuizRepository, QuizRepository>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quiz Generator API v1");
        c.RoutePrefix = "swagger"; // access at /swagger
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
