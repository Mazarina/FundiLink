using FundiLink.Application.Features.Bursaries.Queries.GetBursaries;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaryById;

public record GetBursaryByIdQuery(Guid Id) : IRequest<BursaryDto?>;
