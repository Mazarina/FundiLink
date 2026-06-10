using FundiLink.Application.Features.Career.Queries.GetCareerOpportunities;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerOpportunityById;

public record GetCareerOpportunityByIdQuery(Guid Id) : IRequest<CareerOpportunityDto?>;
