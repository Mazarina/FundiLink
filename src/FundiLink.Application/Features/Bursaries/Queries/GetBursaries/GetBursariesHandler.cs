using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaries;

public class GetBursariesHandler : IRequestHandler<GetBursariesQuery, IEnumerable<BursaryDto>>
{
    private readonly IBursaryRepository _bursaryRepository;

    public GetBursariesHandler(IBursaryRepository bursaryRepository)
    {
        _bursaryRepository = bursaryRepository;
    }

    public async Task<IEnumerable<BursaryDto>> Handle(GetBursariesQuery request, CancellationToken cancellationToken)
    {
        var bursaries = await _bursaryRepository.GetActiveAsync(
            request.FieldOfStudy, request.Province, request.FundingType, cancellationToken);

        return bursaries.Select(ToDto).ToList();
    }

    internal static BursaryDto ToDto(Bursary b) => new(
        b.Id,
        b.Name,
        b.ProviderName,
        b.Description,
        b.FundingType,
        b.FieldsOfStudy,
        b.MinimumAps,
        b.MaxHouseholdIncome,
        b.ProvincesEligible,
        b.ApplicationOpenDate,
        b.ApplicationCloseDate,
        b.ExternalApplicationUrl,
        BursaryDisclaimer.Text);
}
