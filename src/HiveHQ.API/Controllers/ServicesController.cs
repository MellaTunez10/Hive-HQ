using HiveHQ.Application.Interfaces;
using HiveHQ.Domain.Entities;
using Microsoft.AspNetCore.Mvc; // This is the crucial one for ControllerBase and HttpGet
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IGenericRepository<BusinessService> _repo;

    public ServicesController(IGenericRepository<BusinessService> repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessService>>> GetServices()
    {
        return Ok(await _repo.GetAllAsync());
    }
}
