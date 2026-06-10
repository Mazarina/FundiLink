using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaries;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaryById;

public class GetBursaryByIdHandler : IRequestHandler<GetBursaryByIdQuery, BursaryDto?>
{
    private readonly IBursaryRepository _bursaryRepository;

    public GetBursaryByIdHandler(IBursaryRepository bursaryRepository)
    {
        _bursaryRepository = bursaryRepository;
    }

    public async Task<BursaryDto?> Handle(GetBursaryByIdQuery request, CancellationToken cancellationToken)
    {
        var bursary = await _bursaryRepository.GetByIdAsync(request.Id, cancellationToken);
        return bursary is null ? null : GetBursariesHandler.ToDto(bursary);
    }
}
