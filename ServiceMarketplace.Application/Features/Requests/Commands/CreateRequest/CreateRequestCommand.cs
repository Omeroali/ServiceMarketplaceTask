using MediatR;
using ServiceMarketplace.Application.Features.Requests.DTOs;

namespace ServiceMarketplace.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommand : IRequest<Guid>
{
    public CreateRequestDto Request { get; set; } = new();

    public string CustomerId { get; set; } = string.Empty;
}