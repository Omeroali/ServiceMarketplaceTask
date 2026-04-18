using MediatR;

namespace ServiceMarketplace.Application.Features.Requests.Commands.CompleteRequest;

public class CompleteRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
}