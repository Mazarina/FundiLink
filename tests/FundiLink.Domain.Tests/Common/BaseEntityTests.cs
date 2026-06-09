using FluentAssertions;
using FundiLink.Domain.Common;

namespace FundiLink.Domain.Tests.Common;

public class BaseEntityTests
{
    private class ConcreteEntity : BaseEntity { }

    [Fact]
    public void NewEntity_ShouldHaveNonEmptyId()
    {
        var entity = new ConcreteEntity();
        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewEntity_ShouldNotBeDeleted()
    {
        var entity = new ConcreteEntity();
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        var entity = new ConcreteEntity();
        entity.SoftDelete();
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
    }
}
