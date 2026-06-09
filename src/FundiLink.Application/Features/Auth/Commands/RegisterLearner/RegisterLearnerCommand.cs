using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Auth.Commands.RegisterLearner;

public record RegisterLearnerCommand(
    string Email,
    string Password,
    string FirstName,
    string Surname,
    DateOnly DateOfBirth,
    string MobileNumber,
    string Province,
    string SchoolName,
    string SchoolProvince,
    GradeLevel GradeLevel,
    bool ConsentAccepted
) : IRequest<RegisterLearnerResult>;

public record RegisterLearnerResult(string UserId, string Message);
