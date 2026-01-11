using AutoMapper;
using HiveHQ.Application.DTOs;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveHQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]// This makes the URL: /api/inventory
public class InventoryController : ControllerBase
{
    private readonly IGenericRepository<InventoryItem> _repo;
    private readonly IMapper _mapper;

    public InventoryController(IGenericRepository<InventoryItem> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<InventoryDto>> CreateItem(InventoryDto dto)
    {
        var item = _mapper.Map<InventoryItem>(dto);
        await _repo.AddAsync(item);
        await _repo.SaveChangesAsync();

        var result = _mapper.Map<InventoryDto>(item);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
    {
        var items = await _repo.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<InventoryDto>>(items));
    }
}
