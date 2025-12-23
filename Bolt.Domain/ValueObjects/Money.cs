namespace Bolt.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money()
    {
        Currency = "GEL"; // Default currency
    }

    public Money(decimal amount, string currency = "GEL")
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        Amount = decimal.Round(amount, 2);
        Currency = currency?.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(currency));
    }


    public static Money Zero(string currency = "GEL") => new Money(0, currency);

    public bool Equals(Money? other) =>
        other != null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => Equals(obj as Money);
    public override string ToString() => $"{Amount:F2} {Currency}";
}
