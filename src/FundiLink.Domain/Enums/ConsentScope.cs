namespace FundiLink.Domain.Enums;

/// <summary>
/// The minimised slice of learner data a consent permits a guardian to view.
/// Data minimisation per POPIA — a guardian never sees more than the consented scope.
/// </summary>
public enum ConsentScope
{
    /// <summary>Basic profile only (name, grade, school, province, completeness).</summary>
    ProfileBasic,

    /// <summary>Basic profile plus application tracking summaries (no documents).</summary>
    ProfileAndApplications
}
