using HiveHQ.Domain.Common;
namespace HiveHQ.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public decimal UnitPrice { get; set; }
    public string Category { get; set; } = string.Empty; // e.g., Stationery, Tech, Snacks
}
