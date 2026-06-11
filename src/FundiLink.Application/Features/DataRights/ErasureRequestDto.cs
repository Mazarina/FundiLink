using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Features.DataRights;

/// <summary>Typed view of an erasure request shared by learner and admin queries.</summary>
public record ErasureRequestDto(
    Guid Id,
    Guid LearnerId,
    ErasureRequestStatus Status,
    string? Reason,
    DateTime RequestedAt,
    DateTime? ReviewedAt,
    string? ReviewNote,
    DateTime? FulfilledAt
)
{
    public static ErasureRequestDto From(ErasureRequest r) => new(
        r.Id, r.LearnerId, r.Status, r.Reason, r.RequestedAt, r.ReviewedAt, r.ReviewNote, r.FulfilledAt);
}
