namespace ServiceMarketplace.Application.Features.Requests.DTOs;

public class CreateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}