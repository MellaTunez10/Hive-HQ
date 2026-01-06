using System;
namespace HiveHQ.Domain.Entities
{
    public class Order
    {
        public  Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Cancelled
    }
}
