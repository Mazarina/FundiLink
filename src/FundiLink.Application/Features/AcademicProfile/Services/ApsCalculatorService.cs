using FundiLink.Domain.Entities;

namespace FundiLink.Application.Features.AcademicProfile.Services;

public class ApsCalculatorService
{
    // Standard NSC APS: sum the top subjects excluding Life Orientation.
    // Most universities count 6 subjects (excluding LO). We follow that standard.
    public int CalculateAps(IEnumerable<NscSubjectResult> subjects)
    {
        var eligible = subjects
            .Where(s => !s.IsLifeOrientation)
            .OrderByDescending(s => s.ApsPoints)
            .Take(6)
            .ToList();

        return eligible.Sum(s => s.ApsPoints);
    }

    public static int PercentageToApsPoints(int percentage) => percentage switch
    {
        >= 80 => 7,
        >= 70 => 6,
        >= 60 => 5,
        >= 50 => 4,
        >= 40 => 3,
        >= 30 => 2,
        _ => 1
    };
}
