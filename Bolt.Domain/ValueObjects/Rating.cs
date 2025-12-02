namespace Bolt.Domain.ValueObjects;

public sealed class Rating : IEquatable<Rating>
{
    public double Value { get; }

    public Rating(double value)
    {
        if (value < 0.0 || value > 5.0) throw new ArgumentOutOfRangeException(nameof(value));
        Value = Math.Round(value, 1);
    }

    public bool Equals(Rating? other) => other != null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as Rating);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString("0.0");
}
