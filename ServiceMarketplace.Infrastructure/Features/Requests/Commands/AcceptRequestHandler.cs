using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.Features.Requests.Commands.AcceptRequest;
using ServiceMarketplace.Domain.Enums;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Features.Requests.Commands;

public class AcceptRequestHandler : IRequestHandler<AcceptRequestCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public AcceptRequestHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AcceptRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ServiceRequests
            .FirstOrDefaultAsync(x => x.Id == request.RequestId);

        if (entity == null)
            throw new Exception("Request not found");

        if (entity.Status != RequestStatus.Pending)
            throw new Exception("Request already taken");

        entity.ProviderId = request.ProviderId;
        entity.Status = RequestStatus.Accepted;

        await _context.SaveChangesAsync();

        return true;
    }
}