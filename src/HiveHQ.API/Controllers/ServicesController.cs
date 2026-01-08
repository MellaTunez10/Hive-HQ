using HiveHQ.Domain.Interfaces;
using HiveHQ.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // This is the crucial one for ControllerBase and HttpGet
using AutoMapper;
using HiveHQ.Domain.DTOs;


[Authorize]
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
    [AllowAnonymous] // Anyone can see the list of services!
    public async Task<ActionResult<IReadOnlyList<ServiceDto>>> GetServices()
    {
        var services = await _repo.GetAllAsync();

        // Map the list of Entities to a list of DTOs
        return Ok(_mapper.Map<IReadOnlyList<ServiceDto>>(services));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> CreateService(ServiceDto ServiceDto)
    {
        // Map DTO to Entity to save in DB
        var serviceEntity = _mapper.Map<BusinessService>(ServiceDto);


        // Add to Database via Repository
        await _repo.AddAsync(serviceEntity);
        await _repo.SaveChangesAsync();

        // Map Entity back to DTO to return to user
        var resultDto = _mapper.Map<ServiceDto>(serviceEntity);

        return CreatedAtAction(nameof(GetServices), new { id = resultDto.Id }, resultDto);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, TopServiceDto topServiceDto)
    {
        var service = await _repo.GetByIdAsync(id);
        if (service == null) return NotFound();

        // Map the updated values from the DTO to the existing entity
        _mapper.Map(topServiceDto, service);

        _repo.Update(service);
        await _repo.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var service = await _repo.GetByIdAsync(id);
        if (service == null) return NotFound();

        _repo.Delete(service);
        await _repo.SaveChangesAsync();

        return NoContent();
    }
}

