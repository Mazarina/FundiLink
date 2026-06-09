using FundiLink.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence;

public class FundiLinkDbContext : IdentityDbContext, IApplicationDbContext
{
    public FundiLinkDbContext(DbContextOptions<FundiLinkDbContext> options) : base(options)
    {
    }

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
