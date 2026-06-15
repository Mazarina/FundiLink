namespace FundiLink.Application.Common;

/// <summary>
/// Shared validation rules for uploaded/replaced documents. Centralised so the upload
/// and replace handlers enforce identical content-type, extension and size limits.
/// </summary>
public static class DocumentValidation
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    /// <summary>Allowed MIME types for the MVP: PDF, images, and Word documents.</summary>
    public static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    /// <summary>Allowed file extensions, matched case-insensitively against the original filename.</summary>
    public static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx"
    };

    public static void Validate(string fileName, string contentType, long sizeBytes)
    {
        if (sizeBytes <= 0 || sizeBytes > MaxSizeBytes)
            throw new InvalidOperationException("File must be larger than 0 bytes and 10 MB or smaller.");

        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException("Only PDF, JPG, PNG, DOC, and DOCX files are allowed.");

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only PDF, JPG, PNG, DOC, and DOCX files are allowed.");
    }
}
