using Microsoft.AspNetCore.Mvc;
using Tuvi.Api.Models;
using Tuvi.Api.Services;

namespace Tuvi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalityController : ControllerBase
{
    private readonly PersonalityService _service;

    public PersonalityController(PersonalityService service) => _service = service;

    /// <summary>Bộ câu hỏi trắc nghiệm 16 nhóm tính cách.</summary>
    [HttpGet("questions")]
    public ActionResult<IEnumerable<PersonalityQuestion>> Questions() => Ok(_service.GetQuestions());

    /// <summary>Chấm kết quả từ danh sách câu trả lời (mỗi câu là 1 ký tự pole: E/I, S/N, T/F, J/P).</summary>
    [HttpPost("result")]
    public ActionResult<PersonalityResult> Result([FromBody] PersonalityRequest req)
    {
        if (req.Answers is null || req.Answers.Count == 0)
            return BadRequest("Cần ít nhất một câu trả lời.");
        return _service.Evaluate(req.Answers);
    }
}
