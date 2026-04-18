using AutoMapper;
using ServiceMarketplace.Application.Features.Requests.DTOs;
using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.Mappings.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<CreateRequestDto, ServiceRequest>();
    }
}