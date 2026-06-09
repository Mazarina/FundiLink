using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Infrastructure.Persistence.Seed;

// DISCLAIMER: Data is for guidance only. Not official requirements.
// These institutions, programmes and APS values are illustrative samples used to
// demonstrate the matching feature. They do NOT represent official, current, or
// guaranteed admission requirements. Learners must always verify with the official
// institution before applying. FundiLink is not an official admissions portal.
public static class ProgrammeSeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FundiLinkDbContext>();

        if (await db.Institutions.AnyAsync())
            return;

        var institutions = BuildInstitutions();
        await db.Institutions.AddRangeAsync(institutions);

        var programmes = BuildProgrammes(institutions);
        await db.Programmes.AddRangeAsync(programmes);

        await db.SaveChangesAsync();
    }

    private static List<Institution> BuildInstitutions()
    {
        return
        [
            Institution.Create("University of Cape Town", InstitutionType.University, "Western Cape", "https://www.uct.ac.za"),
            Institution.Create("University of the Witwatersrand", InstitutionType.University, "Gauteng", "https://www.wits.ac.za"),
            Institution.Create("University of KwaZulu-Natal", InstitutionType.University, "KwaZulu-Natal", "https://www.ukzn.ac.za"),
            Institution.Create("University of Pretoria", InstitutionType.University, "Gauteng", "https://www.up.ac.za"),
            Institution.Create("University of Limpopo", InstitutionType.University, "Limpopo", "https://www.ul.ac.za"),
            Institution.Create("False Bay TVET College", InstitutionType.TVET, "Western Cape", "https://www.falsebaycollege.co.za"),
            Institution.Create("Ekurhuleni East TVET College", InstitutionType.TVET, "Gauteng", "https://www.eec.edu.za"),
            Institution.Create("Capricorn TVET College", InstitutionType.TVET, "Limpopo", "https://www.capricorncollege.edu.za"),
            Institution.Create("ZulTek Skills Development Centre", InstitutionType.SkillsCentre, "KwaZulu-Natal"),
        ];
    }

    private static List<Programme> BuildProgrammes(List<Institution> institutions)
    {
        var uct = institutions[0].Id;
        var wits = institutions[1].Id;
        var ukzn = institutions[2].Id;
        var up = institutions[3].Id;
        var ul = institutions[4].Id;
        var falseBay = institutions[5].Id;
        var ekurhuleni = institutions[6].Id;
        var capricorn = institutions[7].Id;
        var zultek = institutions[8].Id;

        return
        [
            Programme.Create(uct, "Bachelor of Science in Computer Science", 42,
                [new RequiredSubject("Mathematics", 70), new RequiredSubject("English Home Language", 60)],
                "Faculty of Science", 7),

            Programme.Create(uct, "Bachelor of Medicine and Surgery (MBChB)", 48,
                [new RequiredSubject("Mathematics", 70), new RequiredSubject("Physical Sciences", 70), new RequiredSubject("Life Sciences", 70)],
                "Faculty of Health Sciences", 8),

            Programme.Create(wits, "Bachelor of Engineering (Electrical)", 43,
                [new RequiredSubject("Mathematics", 70), new RequiredSubject("Physical Sciences", 60)],
                "Faculty of Engineering and the Built Environment", 7),

            Programme.Create(wits, "Bachelor of Commerce in Accounting", 38,
                [new RequiredSubject("Mathematics", 60), new RequiredSubject("English Home Language", 50)],
                "Faculty of Commerce, Law and Management", 7),

            Programme.Create(ukzn, "Bachelor of Education (Foundation Phase)", 30,
                [new RequiredSubject("English Home Language", 50)],
                "School of Education", 7),

            Programme.Create(up, "Bachelor of Laws (LLB)", 35,
                [new RequiredSubject("English Home Language", 60)],
                "Faculty of Law", 8),

            Programme.Create(ul, "Bachelor of Science in Agriculture", 26,
                [new RequiredSubject("Mathematics", 50), new RequiredSubject("Life Sciences", 50)],
                "Faculty of Science and Agriculture", 7),

            Programme.Create(falseBay, "National Certificate (Vocational): Information Technology", 18,
                [new RequiredSubject("Mathematics", 40)],
                nfqLevel: 4),

            Programme.Create(ekurhuleni, "National Certificate (Vocational): Electrical Infrastructure Construction", 18,
                [new RequiredSubject("Mathematics", 40)],
                nfqLevel: 4),

            Programme.Create(capricorn, "Report 191: Engineering Studies (N1-N3)", 16,
                [new RequiredSubject("Mathematics", 40)],
                nfqLevel: 2),

            Programme.Create(zultek, "Coding and Digital Skills Bootcamp", 0,
                [],
                "Skills Development"),
        ];
    }
}
