using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Infrastructure.Persistence.Seed;

// DISCLAIMER: Career opportunity data below is curated, generic example information for guidance
// only. It does NOT represent real, current, open, or guaranteed opportunities. No real employer
// facts, stipends, or contact details are fabricated; entries are deliberately generic. FundiLink
// is not an employer or recruitment agency and does not submit applications or guarantee placement.
public static class CareerSeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FundiLinkDbContext>();

        if (await db.CareerOpportunities.AnyAsync())
            return;

        await db.CareerOpportunities.AddRangeAsync(BuildOpportunities());
        await db.SaveChangesAsync();
    }

    private static List<CareerOpportunity> BuildOpportunities()
    {
        return
        [
            CareerOpportunity.Create(
                "IT support learnership (guidance)",
                "SETA-accredited training provider (generic example)",
                "Example of an NQF-aligned IT support learnership for matriculants. Guidance only — apply and " +
                "confirm all details on the provider's official channel.",
                CareerOpportunityType.Learnership,
                fieldsOfInterest: ["Information Technology", "Computer Science"],
                minimumGradeLevel: GradeLevel.Grade12,
                provincesEligible: []),
            CareerOpportunity.Create(
                "Business administration internship (guidance)",
                "Employer (generic example)",
                "Example of an entry-level business administration internship. Guidance only — verify the " +
                "opportunity and apply directly with the provider.",
                CareerOpportunityType.Internship,
                fieldsOfInterest: ["Business", "Commerce"],
                minimumGradeLevel: GradeLevel.PostMatric,
                provincesEligible: ["Gauteng"]),
            CareerOpportunity.Create(
                "Artisan skills programme (guidance)",
                "TVET / SETA programme (generic example)",
                "Example of a hands-on artisan skills programme. Guidance only — confirm intake dates and " +
                "requirements with the provider or a FundiLink support agent.",
                CareerOpportunityType.SkillsProgramme,
                fieldsOfInterest: ["Engineering", "Trades"],
                minimumGradeLevel: GradeLevel.Grade11,
                provincesEligible: []),
            CareerOpportunity.Create(
                "Healthcare assistant learnership (guidance)",
                "Accredited health training provider (generic example)",
                "Example of a healthcare assistant learnership. Guidance only — apply and verify eligibility " +
                "directly with the provider.",
                CareerOpportunityType.Learnership,
                fieldsOfInterest: ["Health Sciences", "Nursing"],
                minimumGradeLevel: GradeLevel.Grade12,
                provincesEligible: ["KwaZulu-Natal"])
        ];
    }
}
