using AutoMapper;
using MediatR;
using ServiceMarketplace.Application.Interfaces;
using ServiceMarketplace.Domain.Entities;

namespace ServiceMarketplace.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestHandler : IRequestHandler<CreateRequestCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;


    public CreateRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<Guid> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.Description))
        {
            request.Request.Description = GenerateDescription(request.Request.Title);
        }
        var entity = _mapper.Map<ServiceRequest>(request.Request);

        entity.CustomerId = request.CustomerId;


        await _authService.CheckUserLimit(request.CustomerId);
        await _unitOfWork.Repository<ServiceRequest>().AddAsync(entity);

        await _unitOfWork.SaveChangesAsync();

        return entity.Id;
    }
    private string GenerateDescription(string title)
    {
        return $"Hi Provider This is a service request related to '{title}'. Please help me ^_^ .";
    }
}