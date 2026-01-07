using HiveHQ.Domain.Common;

namespace HiveHQ.Domain.Entities;

public class Order : BaseEntity
{
    // Link to the Service
    public Guid BusinessServiceId { get; set; }
    public BusinessService BusinessService { get; set; } = null!;

    // Link to the Staff member who made the sale
    public Guid StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public string CustomerName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
}
