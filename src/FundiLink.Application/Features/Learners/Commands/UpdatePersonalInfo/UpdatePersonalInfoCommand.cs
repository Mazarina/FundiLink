using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Learners.Commands.UpdatePersonalInfo;

public record UpdatePersonalInfoCommand(
    string UserId,
    string FirstName,
    string Surname,
    string? IdNumber,
    string? PassportNumber,
    string? Gender,
    string? HomeLanguage,
    string Nationality,
    string MobileNumber,
    string Province,
    string Municipality,
    string Suburb,
    string SchoolName,
    string SchoolProvince,
    GradeLevel GradeLevel,
    string? GuardianName,
    string? GuardianPhone,
    string? GuardianEmail
) : IRequest;
