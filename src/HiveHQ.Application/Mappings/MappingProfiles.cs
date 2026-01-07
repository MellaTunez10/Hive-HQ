using AutoMapper;
using HiveHQ.Application.DTOs;
using HiveHQ.Domain.Entities;

namespace HiveHQ.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Entity -> DTO
        CreateMap<BusinessService, ServiceDto>();

        // DTO -> Entity (For creating new services)
        CreateMap<ServiceDto, BusinessService>();
    }
}

