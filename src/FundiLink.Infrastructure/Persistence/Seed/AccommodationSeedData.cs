using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Infrastructure.Persistence.Seed;

// DISCLAIMER: Accommodation data below is curated, generic example information for guidance
// only. It does NOT represent real, current, available, or guaranteed accommodation. No real
// provider facts, prices, contact details, or addresses are fabricated; entries are deliberately
// generic. FundiLink is not an accommodation provider, landlord, or booking agent. Learners must
// verify everything directly with a provider before committing.
public static class AccommodationSeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FundiLinkDbContext>();

        if (await db.AccommodationListings.AnyAsync())
            return;

        await db.AccommodationListings.AddRangeAsync(BuildListings());
        await db.SaveChangesAsync();
    }

    private static List<AccommodationListing> BuildListings()
    {
        return
        [
            AccommodationListing.Create(
                "Campus-accredited student residence (guidance)",
                "University-accredited residence (generic example)",
                "Example of an accredited student residence close to a public university. Guidance only — " +
                "verify availability, price, and accreditation directly with the institution or provider.",
                AccommodationType.PrivateStudentResidence,
                province: "Gauteng",
                city: "Johannesburg",
                nearInstitution: "University of Johannesburg"),
            AccommodationListing.Create(
                "Shared student house near campus (guidance)",
                "Private landlord (generic example)",
                "Example of a shared student house arrangement. Guidance only — confirm all terms, deposits, " +
                "and safety in person before committing. FundiLink does not handle bookings or payments.",
                AccommodationType.SharedHouse,
                province: "Western Cape",
                city: "Cape Town",
                nearInstitution: "University of Cape Town"),
            AccommodationListing.Create(
                "On-campus residence (guidance)",
                "Public university residence office (generic example)",
                "Example of on-campus residence offered through a university residence office. Guidance only — " +
                "apply and confirm placement through the official university residence process.",
                AccommodationType.ResidenceOnCampus,
                province: "KwaZulu-Natal",
                city: "Durban",
                nearInstitution: "University of KwaZulu-Natal"),
            AccommodationListing.Create(
                "Single room near TVET college (guidance)",
                "Private landlord (generic example)",
                "Example of a single room near a TVET college. Guidance only — verify the listing, terms, and " +
                "safety directly with the provider or talk to a FundiLink support agent.",
                AccommodationType.Room,
                province: "Limpopo",
                city: "Polokwane",
                nearInstitution: "Capricorn TVET College")
        ];
    }
}
