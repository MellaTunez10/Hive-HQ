using AutoMapper;
using HiveHQ.Domain.DTOs;
using HiveHQ.Domain.Entities;
using HiveHQ.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveHQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IGenericRepository<Order> _orderRepo;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<InventoryItem> _inventoryRepo;
    private readonly IGenericRepository<BusinessService> _serviceRepo;

    public OrdersController(
        IGenericRepository<Order> orderRepo,
        IGenericRepository<InventoryItem> inventoryRepo,
        IGenericRepository<BusinessService> serviceRepo,
        IMapper mapper)

    {
        _orderRepo = orderRepo;
        _inventoryRepo = inventoryRepo;
        _serviceRepo = serviceRepo;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto orderDto)
    {
        // 1. Get the Service (to get the official Price)
        var service = await _serviceRepo.GetByIdAsync(orderDto.BusinessServiceId);
        if (service == null) return NotFound("Service not found");

        // 2. The Inventory Guardrail
        if (service.InventoryItemId.HasValue)
        {
            var item = await _inventoryRepo.GetByIdAsync(service.InventoryItemId.Value);
            if (item == null || item.QuantityInStock <= 0)
            {
                return BadRequest($"Item '{item?.Name}' is out of stock.");
            }

            // Deduct stock
            item.QuantityInStock--;
            _inventoryRepo.Update(item);
        }

        // 3. Map DTO to Entity
        var order = _mapper.Map<Order>(orderDto);

        // 4. Force business rules
        order.TotalPrice = service.Price; // The user can't choose their own price
        order.CreatedAt = DateTime.UtcNow;

        await _orderRepo.AddAsync(order);
        await _orderRepo.SaveChangesAsync(); // Saves both Order and Inventory update

        return Ok(_mapper.Map<OrderDto>(order));
    }

     [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(_mapper.Map<OrderDto>(order));
    }
}
