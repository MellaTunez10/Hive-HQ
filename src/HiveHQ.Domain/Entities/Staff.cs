using HiveHQ.Domain.Common;
namespace HiveHQ.Domain.Entities;


public class Staff : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Staff"; // e.g., Admin, Manager, Receptionist
    public bool IsActive { get; set; } = true;
}

