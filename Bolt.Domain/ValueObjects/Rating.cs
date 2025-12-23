namespace Bolt.Domain.ValueObjects;

public sealed class Rating : IEquatable<Rating>
{
    public double Value { get; }
    public int TotalReviews { get; }

    private Rating() { } // EF Core

    public Rating(double value, int totalReviews = 1)
    {
        if (value < 0.0 || value > 5.0)
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 0 and 5.");

        if (totalReviews < 0)
            throw new ArgumentOutOfRangeException(nameof(totalReviews), "Total reviews cannot be negative.");

        Value = Math.Round(value, 1);
        TotalReviews = totalReviews;
    }

    public Rating UpdateWith(double newRatingValue)
    {
        if (newRatingValue < 0.0 || newRatingValue > 5.0)
            throw new ArgumentOutOfRangeException(nameof(newRatingValue));

        var newTotal = TotalReviews + 1;
        var newAverage = ((Value * TotalReviews) + newRatingValue) / newTotal;
        return new Rating(newAverage, newTotal);
    }

    public bool Equals(Rating? other) =>
        other != null && Math.Abs(Value - other.Value) < 0.01 && TotalReviews == other.TotalReviews;

    public override bool Equals(object? obj) => Equals(obj as Rating);
    public override string ToString() => $"{Value:F1} ({TotalReviews} reviews)";
}
