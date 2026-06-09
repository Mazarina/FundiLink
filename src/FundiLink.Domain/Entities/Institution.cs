using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class Institution : BaseEntity
{
    public string Name { get; private set; } = default!;
    public InstitutionType InstitutionType { get; private set; }
    public string Province { get; private set; } = default!;
    public string? Website { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Programme> _programmes = [];
    public IReadOnlyCollection<Programme> Programmes => _programmes.AsReadOnly();

    private Institution() { }

    public static Institution Create(
        string name,
        InstitutionType institutionType,
        string province,
        string? website = null,
        bool isActive = true)
    {
        return new Institution
        {
            Name = name,
            InstitutionType = institutionType,
            Province = province,
            Website = website,
            IsActive = isActive
        };
    }

    public void Update(
        string name,
        InstitutionType institutionType,
        string province,
        string? website,
        bool isActive)
    {
        Name = name;
        InstitutionType = institutionType;
        Province = province;
        Website = website;
        IsActive = isActive;
        MarkUpdated();
    }
}
