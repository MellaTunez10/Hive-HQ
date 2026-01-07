namespace HiveHQ.Application.DTOs;

public class InventoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public decimal UnitPrice { get; set; }
}
