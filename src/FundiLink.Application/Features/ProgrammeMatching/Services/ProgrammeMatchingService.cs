using FundiLink.Domain.Entities;

namespace FundiLink.Application.Features.ProgrammeMatching.Services;

public class ProgrammeMatch
{
    public Programme Programme { get; init; } = default!;
    public string InstitutionName { get; init; } = default!;
    public bool IsEligible { get; init; }
    public int ApsGap { get; init; } // 0 if eligible
    public List<string> MissingSubjects { get; init; } = [];
}

public class ProgrammeMatchingService
{
    // DISCLAIMER: Matching is guidance only and based on data that may not reflect
    // official, current institution requirements. Learners must always verify with
    // the official institution before applying.
    public IEnumerable<ProgrammeMatch> GetMatchingProgrammes(
        Domain.Entities.AcademicProfile profile,
        IEnumerable<(Programme Programme, string InstitutionName)> programmes)
    {
        foreach (var (programme, institutionName) in programmes)
        {
            var apsGap = Math.Max(0, programme.MinimumAps - profile.ApsScore);
            var apsMet = profile.ApsScore >= programme.MinimumAps;

            var missingSubjects = GetMissingSubjects(profile, programme);

            yield return new ProgrammeMatch
            {
                Programme = programme,
                InstitutionName = institutionName,
                IsEligible = apsMet && missingSubjects.Count == 0,
                ApsGap = apsGap,
                MissingSubjects = missingSubjects
            };
        }
    }

    private static List<string> GetMissingSubjects(Domain.Entities.AcademicProfile profile, Programme programme)
    {
        var missing = new List<string>();

        foreach (var required in programme.RequiredSubjects)
        {
            // Life Orientation is never used as a programme entry requirement blocker.
            if (string.Equals(required.SubjectName, "Life Orientation", StringComparison.OrdinalIgnoreCase))
                continue;

            var learnerSubject = profile.Subjects.FirstOrDefault(s =>
                string.Equals(s.SubjectName, required.SubjectName, StringComparison.OrdinalIgnoreCase));

            if (learnerSubject is null || learnerSubject.Percentage < required.MinimumPercentage)
                missing.Add(required.SubjectName);
        }

        return missing;
    }
}
