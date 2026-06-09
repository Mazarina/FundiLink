using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class AcademicProfile : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public int Year { get; private set; }
    public ResultType ResultType { get; private set; }
    public int ApsScore { get; private set; }
    public DateTime? ApsCalculatedAt { get; private set; }

    private readonly List<NscSubjectResult> _subjects = [];
    public IReadOnlyCollection<NscSubjectResult> Subjects => _subjects.AsReadOnly();

    // Navigation
    public Learner Learner { get; private set; } = default!;

    private AcademicProfile() { }

    public static AcademicProfile Create(Guid learnerId, int year, ResultType resultType)
    {
        return new AcademicProfile
        {
            LearnerId = learnerId,
            Year = year,
            ResultType = resultType
        };
    }

    public void SetSubjects(IEnumerable<NscSubjectResult> subjects)
    {
        _subjects.Clear();
        _subjects.AddRange(subjects);
        MarkUpdated();
    }

    public void SetApsScore(int apsScore)
    {
        ApsScore = apsScore;
        ApsCalculatedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}
