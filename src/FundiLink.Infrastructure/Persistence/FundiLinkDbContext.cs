using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence;

public class FundiLinkDbContext : IdentityDbContext, IApplicationDbContext
{
    public FundiLinkDbContext(DbContextOptions<FundiLinkDbContext> options) : base(options)
    {
    }

    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<AcademicProfile> AcademicProfiles => Set<AcademicProfile>();
    public DbSet<NscSubjectResult> NscSubjectResults => Set<NscSubjectResult>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<Programme> Programmes => Set<Programme>();
    public DbSet<LearnerApplication> LearnerApplications => Set<LearnerApplication>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChecklistItem> DocumentChecklistItems => Set<DocumentChecklistItem>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(FundiLinkDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
