namespace Bolt.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { } // EF Core

    public Money(decimal amount, string currency = "GEL")
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        Amount = decimal.Round(amount, 2);
        Currency = currency?.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(currency));
    }

    public Money Add(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (other.Currency != Currency)
            throw new InvalidOperationException($"Cannot add {other.Currency} to {Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (other.Currency != Currency)
            throw new InvalidOperationException($"Cannot subtract {other.Currency} from {Currency}");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0) throw new ArgumentOutOfRangeException(nameof(factor));
        return new Money(Amount * factor, Currency);
    }

    public static Money Zero(string currency = "USD") => new Money(0, currency);

    public bool Equals(Money? other) =>
        other != null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => Equals(obj as Money);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount:F2} {Currency}";
}
