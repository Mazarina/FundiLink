using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class Document : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; } = default!;
    public DocumentStatus Status { get; private set; } = DocumentStatus.Pending;
    public string? RejectionReason { get; private set; }
    public string? VerifiedByUserId { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    private Document() { }

    public static Document Create(
        Guid learnerId,
        DocumentType documentType,
        string fileName,
        string contentType,
        long sizeBytes,
        string storageKey)
    {
        return new Document
        {
            LearnerId = learnerId,
            DocumentType = documentType,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
            Status = DocumentStatus.Pending
        };
    }

    public void Verify(string verifiedByUserId)
    {
        Status = DocumentStatus.Verified;
        VerifiedByUserId = verifiedByUserId;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = null;
        MarkUpdated();
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A rejection reason is required.");

        Status = DocumentStatus.Rejected;
        RejectionReason = reason;
        VerifiedByUserId = null;
        VerifiedAt = null;
        MarkUpdated();
    }

    /// <summary>
    /// Replaces the file behind this document record (same storage key, new content).
    /// Resets verification state so the replacement is re-checked by an admin.
    /// </summary>
    public void ReplaceFile(string fileName, string contentType, long sizeBytes)
    {
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Status = DocumentStatus.Pending;
        RejectionReason = null;
        VerifiedByUserId = null;
        VerifiedAt = null;
        MarkUpdated();
    }
}
