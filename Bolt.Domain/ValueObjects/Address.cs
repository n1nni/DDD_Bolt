namespace Bolt.Domain.ValueObjects;

public sealed class Address : IEquatable<Address>
{
    public string Description { get; }
    public Location Location { get; }

    public Address(string description, Location location)
    {
        Description = description?.Trim() ?? throw new ArgumentNullException(nameof(description));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Address description cannot be empty.");
    }

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        return Description == other.Description && Location.Equals(other.Location);
    }

    public override bool Equals(object? obj) => Equals(obj as Address);
    public override int GetHashCode() => HashCode.Combine(Description, Location);
    public override string ToString() => $"{Description} ({Location})";
}
