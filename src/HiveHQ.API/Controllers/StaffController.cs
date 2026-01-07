using AutoMapper;
using HiveHQ.Application.DTOs;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveHQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IGenericRepository<Staff> _staffRepo;
    private readonly IMapper _mapper;

    public StaffController(IGenericRepository<Staff> staffRepo, IMapper mapper)
    {
        _staffRepo = staffRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StaffDto>>> GetStaff()
    {
        var staff = await _staffRepo.GetAllAsync();
        return Ok(_mapper.Map<IReadOnlyList<StaffDto>>(staff));
    }

    [HttpPost]
    public async Task<ActionResult<StaffDto>> CreateStaff(StaffDto staffDto)
    {
        var staff = _mapper.Map<Staff>(staffDto);
        await _staffRepo.AddAsync(staff);
        await _staffRepo.SaveChangesAsync();

        var result = _mapper.Map<StaffDto>(staff);
        return CreatedAtAction(nameof(GetStaff), result);
    }
}
