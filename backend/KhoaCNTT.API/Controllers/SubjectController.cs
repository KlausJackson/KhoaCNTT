
using KhoaCNTT.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectRepository _repo;

    public SubjectsController(ISubjectRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _repo.GetAllAsync();
        return Ok(subjects);
    }
}