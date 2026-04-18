using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.Features.Requests.DTOs;
using ServiceMarketplace.Application.Features.Requests.Queries;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Features.Requests.Queries;

public class GetRequestsHandler : IRequestHandler<GetRequestsQuery, List<ServiceRequestDto>>
{
    private readonly ApplicationDbContext _context;

    public GetRequestsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceRequestDto>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ServiceRequests.AsQueryable();

        // Admin
        if (request.IsAdmin)
        {
            return await query.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Status = (int)x.Status 

            }).ToListAsync();
        }

        //  Customer
        if (!request.IsProvider)
        {
            return await query
                .Where(x => x.CustomerId == request.UserId)
                .Select(x => new ServiceRequestDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Status = (int)x.Status
                }).ToListAsync();
        }

        // Provider
        var list = await query.ToListAsync();

        return list
            .Where(x => GetDistance(request.Latitude, request.Longitude, x.Latitude, x.Longitude) <= 10)
            .Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Status = (int)x.Status
            }).ToList();
    }

    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private double ToRad(double value)
    {
        return value * Math.PI / 180;
    }
}