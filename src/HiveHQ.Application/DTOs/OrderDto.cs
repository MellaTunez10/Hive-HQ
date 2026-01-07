namespace HiveHQ.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Guid BusinessServiceId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
}
