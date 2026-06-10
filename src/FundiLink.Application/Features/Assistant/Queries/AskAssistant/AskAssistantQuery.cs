using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Assistant.Queries.AskAssistant;

public record AskAssistantQuery(string UserId, AssistantIntent Intent) : IRequest<AssistantResponseDto>;

public record AssistantResponseDto(
    AssistantIntent Intent,
    string Answer,
    IReadOnlyList<string> Sources,
    bool GuidanceOnly,
    string Disclaimer
);
