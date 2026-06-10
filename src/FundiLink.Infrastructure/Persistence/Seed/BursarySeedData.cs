using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Infrastructure.Persistence.Seed;

// DISCLAIMER: Bursary data below is curated PUBLIC information for guidance only.
// It does NOT represent official, current, or guaranteed funding terms. No amounts or
// contact details are fabricated; descriptions are kept generic. FundiLink is not an
// official bursary, NSFAS, or funding platform. Learners must always apply on the
// funder's own official portal and verify details with the funder.
public static class BursarySeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FundiLinkDbContext>();

        if (await db.Bursaries.AnyAsync())
            return;

        await db.Bursaries.AddRangeAsync(BuildBursaries());
        await db.SaveChangesAsync();
    }

    private static List<Bursary> BuildBursaries()
    {
        return
        [
            Bursary.Create(
                "NSFAS Student Funding (guidance)",
                "National Student Financial Aid Scheme (NSFAS)",
                "Government financial aid for qualifying students at public universities and TVET colleges. " +
                "Guidance only — apply and confirm all eligibility on the official NSFAS portal.",
                BursaryFundingType.FullCost,
                fieldsOfStudy: [],
                minimumAps: null,
                provincesEligible: [],
                externalApplicationUrl: "https://www.nsfas.org.za"),

            Bursary.Create(
                "Funza Lushaka Bursary (guidance)",
                "Department of Basic Education",
                "A bursary programme that supports students pursuing teaching qualifications in priority subject areas. " +
                "Guidance only — apply and confirm requirements on the official programme portal.",
                BursaryFundingType.FullCost,
                fieldsOfStudy: ["Education", "Teaching"],
                minimumAps: 26,
                provincesEligible: [],
                externalApplicationUrl: "https://www.funzalushaka.doe.gov.za"),

            Bursary.Create(
                "General Engineering Bursary (guidance)",
                "Curated public listing",
                "Illustrative listing for engineering-field bursaries commonly offered by public and private funders. " +
                "Guidance only — verify any specific bursary with its funder before applying.",
                BursaryFundingType.TuitionOnly,
                fieldsOfStudy: ["Engineering"],
                minimumAps: 38,
                provincesEligible: []),

            Bursary.Create(
                "Provincial Skills Stipend (guidance)",
                "Curated public listing",
                "Illustrative listing for provincial skills-development stipends supporting early-career learners. " +
                "Guidance only — verify availability with the relevant provincial programme.",
                BursaryFundingType.Stipend,
                fieldsOfStudy: [],
                minimumAps: null,
                provincesEligible: ["KwaZulu-Natal", "Limpopo"]),
        ];
    }
}
