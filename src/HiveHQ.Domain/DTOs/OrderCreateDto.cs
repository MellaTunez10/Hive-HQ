namespace HiveHQ.Domain.DTOs;

public class OrderCreateDto
{
    public Guid BusinessServiceId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    // Add any other fields the user fills out when buying,
    // but NOT Id, TotalPrice, or CreatedAt.
}
