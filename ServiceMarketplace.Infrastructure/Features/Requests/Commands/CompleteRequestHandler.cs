using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.Features.Requests.Commands.CompleteRequest;
using ServiceMarketplace.Domain.Enums;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Features.Requests.Commands;

public class CompleteRequestHandler : IRequestHandler<CompleteRequestCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public CompleteRequestHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CompleteRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ServiceRequests
            .FirstOrDefaultAsync(x => x.Id == request.RequestId);

        if (entity == null)
            throw new Exception("Request not found");

        if (entity.Status != RequestStatus.Accepted)
            throw new Exception("Request is not accepted yet");

        if (entity.ProviderId != request.ProviderId)
            throw new Exception("You are not assigned to this request");

        entity.Status = RequestStatus.Completed;

        await _context.SaveChangesAsync();

        return true;
    }
}