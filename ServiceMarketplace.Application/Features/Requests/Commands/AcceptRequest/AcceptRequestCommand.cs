using MediatR;

namespace ServiceMarketplace.Application.Features.Requests.Commands.AcceptRequest;

public class AcceptRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
}