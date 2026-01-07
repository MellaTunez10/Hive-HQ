using System;
using HiveHQ.Domain.Common;

namespace HiveHQ.Domain.Entities;

public class BusinessService : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}
