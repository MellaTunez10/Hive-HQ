using HiveHQ.Application.Interfaces;
using HiveHQ.Domain.Entities;
using Microsoft.AspNetCore.Mvc; // This is the crucial one for ControllerBase and HttpGet
using AutoMapper;
using HiveHQ.Application.DTOs;


[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IGenericRepository<BusinessService> _repo;
    private readonly IMapper _mapper;

    public ServicesController(IGenericRepository<BusinessService> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessService>>> GetServices()
    {
        var services = await _repo.GetAllAsync();

        // Map the list of Entities to a list of DTOs
        return Ok(_mapper.Map<IReadOnlyList<ServiceDto>>(services));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> CreateService(ServiceDto serviceDto)
    {
        // Map DTO to Entity to save in DB
        var serviceEntity = _mapper.Map<BusinessService>(serviceDto);


        // Add to Database via Repository
        await _repo.AddAsync(serviceEntity);
        await _repo.SaveChangesAsync();

        // Map Entity back to DTO to return to user
        var resultDto = _mapper.Map<ServiceDto>(serviceEntity);

        return CreatedAtAction(nameof(GetServices), new { id = resultDto.Id }, resultDto);
    }
}
