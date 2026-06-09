using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class DocumentChecklistItem : BaseEntity
{
    public Guid LearnerApplicationId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public bool IsRequired { get; private set; }
    public Guid? LinkedDocumentId { get; private set; }

    private DocumentChecklistItem() { }

    public static DocumentChecklistItem Create(
        Guid learnerApplicationId,
        DocumentType documentType,
        bool isRequired)
    {
        return new DocumentChecklistItem
        {
            LearnerApplicationId = learnerApplicationId,
            DocumentType = documentType,
            IsRequired = isRequired
        };
    }

    public void LinkDocument(Guid documentId)
    {
        LinkedDocumentId = documentId;
        MarkUpdated();
    }
}
