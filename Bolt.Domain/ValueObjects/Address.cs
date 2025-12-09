namespace Bolt.Domain.ValueObjects;

public sealed class Address : IEquatable<Address>
{
    public string Street { get; }
    public string City { get; }
    public string? PostalCode { get; }
    public Location Location { get; }

    private Address() { } // EF Core

    public Address(string street, string city, Location location, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));

        Street = street.Trim();
        City = city.Trim();
        PostalCode = postalCode?.Trim();
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        return Street == other.Street &&
               City == other.City &&
               Location.Equals(other.Location);
    }

    public override bool Equals(object? obj) => Equals(obj as Address);
    public override int GetHashCode() => HashCode.Combine(Street, City, Location);
    public override string ToString() => $"{Street}, {City} ({Location})";
}
