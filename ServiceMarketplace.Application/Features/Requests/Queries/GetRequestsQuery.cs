using MediatR;
using ServiceMarketplace.Application.Features.Requests.DTOs;

namespace ServiceMarketplace.Application.Features.Requests.Queries;

public class GetRequestsQuery : IRequest<List<ServiceRequestDto>>
{
    public string UserId { get; set; } = string.Empty;

    public bool IsProvider { get; set; }
    public bool IsAdmin { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}