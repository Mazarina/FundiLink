namespace FundiLink.Domain.Enums;

/// <summary>
/// The kinds of guardian consent FundiLink records for a minor learner (under 18).
/// Each consent is captured, append-only, and may be revoked (right to withdraw).
/// </summary>
public enum ConsentType
{
    /// <summary>Consent for FundiLink to process the learner's personal information.</summary>
    DataProcessing,

    /// <summary>Consent allowing a linked guardian read-only co-access to the learner's profile.</summary>
    GuardianCoAccess,

    /// <summary>Consent to share application/profile information with institutions or funders the learner chooses.</summary>
    SharingWithInstitutions
}
