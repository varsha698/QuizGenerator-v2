using Microsoft.AspNetCore.Mvc;
using QuizGeneratorApi.Api.Models;        // <- your Quiz/Question models
using QuizGeneratorApi.Api.MongoRepository;  // <- your IQuizRepository
// using QuizApi.Models; using QuizApi.Repositories; // if you used those names instead

namespace QuizGeneratorApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizRepository _repo;
    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(IQuizRepository repo, ILogger<QuizzesController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET /api/quizzes?page=1&pageSize=20&search=hello&category=General
    [HttpGet(Name = "ListQuizzes")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var (items, total) = await _repo.GetAsync(page, pageSize, search, category);

        return Ok(new
        {
            page,
            pageSize,
            total,
            items
        });
    }

    // GET /api/quizzes/{id}
    [HttpGet("{id}", Name = "GetQuizById")]
    public async Task<IActionResult> GetById(string id)
    {
        var quiz = await _repo.GetByIdAsync(id);
        return quiz is null ? NotFound() : Ok(quiz);
    }

    // POST /api/quizzes
    [HttpPost(Name = "CreateQuiz")]
    public async Task<IActionResult> Create([FromBody] Quiz quiz)
    {
        if (quiz is null) return BadRequest("Quiz payload is required.");

        var id = await _repo.CreateAsync(quiz);
        return CreatedAtRoute("GetQuizById", new { id }, quiz);
    }

    // PUT /api/quizzes/{id}
    [HttpPut("{id}", Name = "UpdateQuiz")]
    public async Task<IActionResult> Update(string id, [FromBody] Quiz quiz)
    {
        if (quiz is null) return BadRequest("Quiz payload is required.");

        var ok = await _repo.UpdateAsync(id, quiz);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/quizzes/{id}
    [HttpDelete("{id}", Name = "DeleteQuiz")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    // ------- Question subresources -------

    // POST /api/quizzes/{id}/questions
    [HttpPost("{id}/questions", Name = "AddQuestion")]
    public async Task<IActionResult> AddQuestion(string id, [FromBody] Question question)
    {
        if (question is null) return BadRequest("Question payload is required.");

        var ok = await _repo.AddQuestionAsync(id, question);
        return ok ? NoContent() : NotFound();
    }

    // PUT /api/quizzes/{id}/questions/{questionId}
    [HttpPut("{id}/questions/{questionId}", Name = "UpdateQuestion")]
    public async Task<IActionResult> UpdateQuestion(string id, string questionId, [FromBody] Question question)
    {
        if (question is null) return BadRequest("Question payload is required.");

        var ok = await _repo.UpdateQuestionAsync(id, questionId, question);
        return ok ? NotFound() : NoContent();
    }

    // DELETE /api/quizzes/{id}/questions/{questionId}
    [HttpDelete("{id}/questions/{questionId}", Name = "RemoveQuestion")]
    public async Task<IActionResult> RemoveQuestion(string id, string questionId)
    {
        var ok = await _repo.RemoveQuestionAsync(id, questionId);
        return ok ? NoContent() : NotFound();
    }
}
