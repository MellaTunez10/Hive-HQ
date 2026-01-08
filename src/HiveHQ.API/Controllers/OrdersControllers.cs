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
    public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
    {
        // 1. Get the service
        var service = await _serviceRepo.GetByIdAsync(orderDto.BusinessServiceId);

        if (service == null) return BadRequest("Service not found");
        Console.WriteLine($"Service found: {service.Name}, InventoryLink: {service.InventoryItemId}");
        // 2. Handle Inventory
        if (service.InventoryItemId.HasValue)
        {
            var inventoryItem = await _inventoryRepo.GetByIdAsync(service.InventoryItemId.Value);
            if (inventoryItem != null)
            {
                if (inventoryItem.QuantityInStock < orderDto.Quantity)
                {
                    string itemName = inventoryItem.Name; // Extract to variable to fix ambiguity
                    return BadRequest($"Not enough stock for {itemName}");
                }
                Console.WriteLine($"SUBTRACTING: {orderDto.Quantity} from {inventoryItem.Name}");
                inventoryItem.QuantityInStock -= orderDto.Quantity;
                _inventoryRepo.Update(inventoryItem);
                // We don't save yet; we wait for the order save to do both at once
            }
        }

        // 3. Map and Save Order
        var order = _mapper.Map<Order>(orderDto);
        await _orderRepo.AddAsync(order);

        // This saves BOTH the inventory reduction and the new order in one transaction
        await _orderRepo.SaveChangesAsync();

        var result = _mapper.Map<OrderDto>(order);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

     [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(_mapper.Map<OrderDto>(order));
    }
}
