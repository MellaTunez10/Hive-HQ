using AutoMapper;
using HiveHQ.Application.DTOs;
using HiveHQ.Domain.Entities;

namespace HiveHQ.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Entity -> DTO
        CreateMap<BusinessService, ServiceDto>().ReverseMap();

        // DTO -> Entity (For creating new services)
        CreateMap<ServiceDto, BusinessService>();

        // Staff mappings
        CreateMap<Staff, StaffDto>().ReverseMap();

        // Inventory mappings
        CreateMap<InventoryItem, InventoryDto>().ReverseMap();

        // Order mappings
        CreateMap<Order, OrderDto>().ReverseMap();
    }
}

