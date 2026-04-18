using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.API.Attributes;
using ServiceMarketplace.Application.Features.Requests.Commands.AcceptRequest;
using ServiceMarketplace.Application.Features.Requests.Commands.CompleteRequest;
using ServiceMarketplace.Application.Features.Requests.Commands.CreateRequest;
using ServiceMarketplace.Application.Features.Requests.DTOs;
using ServiceMarketplace.Application.Features.Requests.Queries;
using System.Security.Claims;


namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(double lat = 0, double lng = 0)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

        if (userId == null)
            return Unauthorized();

        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var query = new GetRequestsQuery
        {
            UserId = userId,
            Latitude = lat,
            Longitude = lng,
            IsProvider = roles.Contains("Provider"),
            IsAdmin = roles.Contains("Admin")
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("request.create")]
    public async Task<IActionResult> Create(CreateRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

        if (userId == null)
            return Unauthorized();

        try
        {
            var command = new CreateRequestCommand
            {
                Request = dto,
                CustomerId = userId
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }



    [HttpPost("{id}/accept")]
    [HasPermission("request.accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub");

        if (providerId == null)
            return Unauthorized();

        try
        {
            var command = new AcceptRequestCommand
            {
                RequestId = id,
                ProviderId = providerId
            };

            await _mediator.Send(command);

            return Ok("Request accepted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }


    [HttpPost("{id}/complete")]
    [HasPermission("request.complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub");

        if (providerId == null)
            return Unauthorized();

        try
        {
            var command = new CompleteRequestCommand
            {
                RequestId = id,
                ProviderId = providerId
            };

            await _mediator.Send(command);

            return Ok("Request completed successfully ✅");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}