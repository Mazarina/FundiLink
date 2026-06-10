using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.DeleteBursaryApplication;

public record DeleteBursaryApplicationCommand(Guid BursaryApplicationId, string UserId) : IRequest;
