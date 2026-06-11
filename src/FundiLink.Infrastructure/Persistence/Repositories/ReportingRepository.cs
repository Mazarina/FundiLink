using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Reporting.Queries.GetOperationsDashboard;
using FundiLink.Application.Features.Reporting.Queries.GetPopiaOperationsSummary;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Deterministic, in-process reporting aggregation over existing data. Read-only and
/// aggregate-first: every method returns counts or grouped totals, never raw learner
/// personal information. No external analytics/telemetry provider is used.
/// </summary>
public class ReportingRepository : IReportingRepository
{
    private readonly FundiLinkDbContext _db;

    public ReportingRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<OperationsDashboardDto> GetOperationsDashboardAsync(CancellationToken ct)
    {
        var totalLearners = await _db.Learners.CountAsync(l => l.DeletedAt == null, ct);

        var provinces = await _db.Learners
            .Where(l => l.DeletedAt == null)
            .Select(l => l.Province)
            .ToListAsync(ct);
        var learnersByProvince = GroupCount(provinces);

        var applicationStatuses = await _db.LearnerApplications
            .Where(a => a.DeletedAt == null)
            .Select(a => a.Status)
            .ToListAsync(ct);
        var applicationsByStatus = GroupCount(applicationStatuses.Select(s => s.ToString()));

        var bursaryStatuses = await _db.BursaryApplications
            .Where(a => a.DeletedAt == null)
            .Select(a => a.Status)
            .ToListAsync(ct);
        var bursaryApplicationsByStatus = GroupCount(bursaryStatuses.Select(s => s.ToString()));

        var documentStatuses = await ActiveDocuments()
            .Select(d => d.Status)
            .ToListAsync(ct);
        var documentsByStatus = GroupCount(documentStatuses.Select(s => s.ToString()));

        var pendingDocumentVerifications = await ActiveDocuments()
            .CountAsync(d => d.Status == DocumentStatus.Pending, ct);

        var pendingErasureRequests = await _db.ErasureRequests
            .CountAsync(r => r.Status == ErasureRequestStatus.Requested, ct);

        var consentGrants = await _db.GuardianConsents
            .CountAsync(c => c.Status == ConsentStatus.Granted, ct);

        var consentRevocations = await _db.GuardianConsents
            .CountAsync(c => c.Status == ConsentStatus.Revoked, ct);

        return new OperationsDashboardDto(
            totalLearners,
            learnersByProvince,
            applicationsByStatus,
            bursaryApplicationsByStatus,
            documentsByStatus,
            pendingDocumentVerifications,
            pendingErasureRequests,
            consentGrants,
            consentRevocations);
    }

    public async Task<PopiaOperationsSummaryDto> GetPopiaOperationsSummaryAsync(CancellationToken ct)
    {
        var pendingDocumentVerifications = await ActiveDocuments()
            .CountAsync(d => d.Status == DocumentStatus.Pending, ct);

        var pendingErasureRequests = await _db.ErasureRequests
            .CountAsync(r => r.Status == ErasureRequestStatus.Requested, ct);

        return new PopiaOperationsSummaryDto(pendingDocumentVerifications, pendingErasureRequests);
    }

    // Documents use a computed soft-delete query filter that the EF InMemory provider cannot
    // translate. Apply the equivalent DeletedAt filter explicitly so reporting is provider-agnostic.
    private IQueryable<Domain.Entities.Document> ActiveDocuments()
        => _db.Documents.IgnoreQueryFilters().Where(d => d.DeletedAt == null);

    private static IReadOnlyList<CountByCategoryDto> GroupCount(IEnumerable<string> values)
        => values
            .GroupBy(v => v)
            .Select(g => new CountByCategoryDto(g.Key, g.Count()))
            .OrderBy(c => c.Category)
            .ToList();
}
