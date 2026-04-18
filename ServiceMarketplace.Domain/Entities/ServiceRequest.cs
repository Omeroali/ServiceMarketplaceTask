using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public string CustomerId { get; set; } = string.Empty;
    public string? ProviderId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}