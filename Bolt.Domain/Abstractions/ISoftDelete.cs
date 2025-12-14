namespace Bolt.Domain.Abstractions;

public interface ISoftDelete
{
    bool IsDeleted { get; }

    void MarkAsDeleted();
    void Restore();
}
