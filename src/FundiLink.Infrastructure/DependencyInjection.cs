using FundiLink.Application.Common.Interfaces;
using FundiLink.Infrastructure.Persistence;
using FundiLink.Infrastructure.Persistence.Repositories;
using FundiLink.Infrastructure.Security;
using FundiLink.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Database connection string 'Default' is not configured.");

        services.AddDbContext<FundiLinkDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<FundiLinkDbContext>());

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false; // false for MVP ease; enable when email is real
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<FundiLinkDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ILearnerRepository, LearnerRepository>();
        services.AddScoped<IProgrammeRepository, ProgrammeRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IEmailService, StubEmailService>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IChecklistRepository, ChecklistRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ILearnerQueryRepository, LearnerQueryRepository>();
        services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        services.AddScoped<IDocumentStorageService, LocalDiskDocumentStorageService>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
        services.AddScoped<IWhatsAppService, StubWhatsAppService>();
        services.AddScoped<ISmsService, StubSmsService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDeadlineQueryRepository, DeadlineQueryRepository>();
        services.AddScoped<IDeadlineReminderService, DeterministicDeadlineReminderService>();
        services.AddScoped<IBursaryRepository, BursaryRepository>();
        services.AddScoped<IBursaryApplicationRepository, BursaryApplicationRepository>();

        // AI guidance assistant — deterministic stub behind the interface. No external LLM
        // call in this phase. A real provider may be wired here later (key via env only).
        services.AddScoped<IAiAssistantService, RuleBasedAiAssistantService>();
        services.AddScoped<IAssistantInteractionLogRepository, AssistantInteractionLogRepository>();

        // Accommodation & early-career opportunities (Phase 8). Curated, seeded data behind
        // repositories — no real external listing/job-board provider integration in this phase.
        // A real provider may be wired later behind the same interface (key via env only).
        services.AddScoped<IAccommodationRepository, AccommodationRepository>();
        services.AddScoped<IAccommodationInterestRepository, AccommodationInterestRepository>();
        services.AddScoped<ICareerRepository, CareerRepository>();
        services.AddScoped<ICareerInterestRepository, CareerInterestRepository>();

        // Guardian consent & co-access (Phase 9). Append-only consent records and guardian
        // links behind repositories. Consent checks are deterministic — no real identity-
        // verification / e-signature provider integration in this phase. A real provider may
        // be wired later behind IConsentCheckService (key via env only).
        services.AddScoped<IGuardianConsentRepository, GuardianConsentRepository>();
        services.AddScoped<IGuardianLinkRepository, GuardianLinkRepository>();
        services.AddScoped<IConsentCheckService, DeterministicConsentCheckService>();

        // Data subject rights — export & erasure (Phase 10, POPIA). Export is generated
        // in-process (typed DTO); erasure fulfilment is a deterministic in-process service
        // that anonymises/soft-deletes personal data while preserving append-only audit and
        // consent records. No third-party storage/email/delivery integration in this phase;
        // a real delivery channel may be wired later behind IErasureService (key via env only).
        services.AddScoped<IErasureRequestRepository, ErasureRequestRepository>();
        services.AddScoped<IErasureService, DeterministicErasureService>();

        // Admin reporting & POPIA operations dashboard (Phase 11). Read-only, aggregate-first
        // reporting computed deterministically in-process behind IReportingRepository. No
        // third-party analytics/telemetry provider; a real provider may be wired later behind
        // the same interface (key via env only). Adds no new way to read learner sensitive fields.
        services.AddScoped<IReportingRepository, ReportingRepository>();

        return services;
    }
}
