using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.DeleteApplication;

public record DeleteApplicationCommand(Guid ApplicationId, string UserId) : IRequest;
